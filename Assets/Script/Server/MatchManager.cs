using System;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using Protocol;
using Battlehub.Dispatcher;
using System.Linq;

public partial class MatchManager : MonoBehaviour
{
    public class MatchInfo
    {
        public string title;
        public string inDate;
        public MatchType matchType;
        public MatchModeType matchModeType;
        public string headCount;
        public bool isSandBoxEnable;
    }

    public Dictionary<SessionId, MatchUserGameRecord> gameRecords { get; private set; } = null;  // 매치에 참가중인 유저들의 매칭 기록
    public MatchType nowMatchType { get; private set; } = MatchType.None;
    public MatchModeType nowModeType { get; private set; } = MatchModeType.None;
    private bool isHost = false;                    // 호스트 여부 (서버에서 설정한 SuperGamer 정보를 가져옴)
    public List<SessionId> SessionIdList { get; private set; }
    private static MatchManager instance = null;
    public SessionId hostSession { get; private set; }  // 호스트 세션

    public bool isConnectMatchServer { get; private set; } = false;
    private bool isJoinGameRoom = false;
    private bool isConnectInGameServer = false;
    private Queue<KeyMessage> localQueue = null;    // 호스트에서 로컬로 처리하는 패킷을 쌓아두는 큐 (로컬처리하는 데이터는 서버로 발송 안함)
    private string inGameRoomToken = string.Empty;  // 게임 룸 토큰 (인게임 접속 토큰)
    public bool isReconnectProcess { get; private set; } = false;
    public bool isSandBoxGame { get; private set; } = false;
    private int numOfClient = 2;                    // 매치에 참가한 유저의 총 수
    public float timer;
    public int Min;

    private bool StartTimer= false;
    public List<MatchInfo> matchInfos { get; private set; } = new List<MatchInfo>();
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }
    public void GetMatchList(Action<bool, string> func)
    {
        matchInfos.Clear();

        Backend.Match.GetMatchList(callback =>
        {
            if (callback.IsSuccess() == false)
            {
                Debug.Log("매칭카드 리스트 불러오기실패 \n" + callback);
                Dispatcher.Current.BeginInvoke(() =>
                {
                    GetMatchList(func);
                });
                return;
            }
            foreach(LitJson.JsonData row in callback.Rows())
            {
                MatchInfo matchInfo = new MatchInfo();
                matchInfo.title = row["matchTitle"]["S"].ToString();
                matchInfo.inDate = row["inDate"]["S"].ToString();
                matchInfo.headCount = row["matchHeadCount"]["N"].ToString();
                matchInfo.isSandBoxEnable = row["enable_sandbox"]["BOOL"].ToString().Equals("True") ? true : false;
                foreach(MatchType type in Enum.GetValues(typeof(MatchType)))
                {
                    if (type.ToString().ToLower().Equals(row["matchType"]["S"].ToString().ToLower()))
                    {
                        matchInfo.matchType = type;
                    }
                }
                foreach(MatchModeType type in Enum.GetValues(typeof(MatchModeType)))
                {
                    if(type.ToString().ToLower().Equals(row["matchModeType"]["S"].ToString().ToLower()))
                    {
                        matchInfo.matchModeType = type;
                    }
                }
                matchInfos.Add(matchInfo);
            }
            Debug.Log("매칭카드 리스트 불러오기 성공 :" + matchInfos.Count());
            func(true, string.Empty);
        });
    }
   
    public static MatchManager GetInstance()
    {
        if (!instance)
        {
            return null;
        }
        return instance;
    }
    private void OnApplicationQuit()
    {
        if (isConnectMatchServer)
        {
            LeaveMatchServer();
            Debug.Log("LeaveMatchServer");
        }
    }
    void Start()
    {
        MatchMakingHandler();
        GameHandler();
        ExceptionHandler();
    }

    // Update is called once per frame
    void Update()
    {
        if (StartTimer)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Min = 0;
        }
        if(timer >60 )
        {
            Min++;
            timer = 0;
        }
        if (isConnectInGameServer || isConnectMatchServer)
        {
            Backend.Match.Poll();

            // 호스트의 경우 로컬 큐가 존재
            // 큐에 있는 패킷을 로컬에서 처리
            if (localQueue != null)
            {
                while (localQueue.Count > 0)
                {
                    var msg = localQueue.Dequeue();
                    // WorldManager.instance.OnRecieveForLocal(msg);
                }
            }
        }
    }

    public void AddMsgToLocalQueue(KeyMessage message)
    {
        if (isHost == false || localQueue == null)
        {
            return;
        }
        localQueue.Enqueue(message);
    }
    private void MatchMakingHandler()
    {
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            ProcessAccessMatchMakingServer(args.ErrInfo);
            CreateMatchRoom_();

        };
        Backend.Match.OnMatchMakingRoomCreate += (args) =>
        {
            Debug.Log("OnMatchMakingCreateRoom");
            RequestMatchMaking(1);
        };
        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            // 매칭 신청 관련 작업에 대한 호출
            Debug.Log("s");

            ProcessMatchMakinRespone(args);
        };
    }

    private void GameHandler()
    {
        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            Debug.Log("OnSessionJoinInserver : " + args.ErrInfo);
            if (args.ErrInfo != ErrorInfo.Success)
            {
                if (isReconnectProcess)
                {
                    if (args.ErrInfo.Reason.Equals("Reconnect Success"))
                    {
                        Debug.Log("재접속 성공");
                    }
                    else if (args.ErrInfo.Reason.Equals("Fail To Reconnect"))
                    {
                        Debug.Log("재접속 실패");
                        JoinMatchServer();
                        isConnectInGameServer = false;
                    }
                }
                return;
            }
            if (isJoinGameRoom)
            {
                return;
            }
            if (inGameRoomToken == string.Empty)
            {
                Debug.LogError("인게임 서버 접속 성공했지만 룸 토큰 없음");
                return;
            }
            Debug.Log("인게임 서버 접속 성공");
            isJoinGameRoom = true;

            AccessInGameRoom(inGameRoomToken);
        };
        Backend.Match.OnSessionListInServer += (args) =>
        {

            Debug.Log("OnSessionListInsServer : " + args.ErrInfo);
            ProcessMatchInGameSessionList(args);
        };

        Backend.Match.OnMatchInGameAccess += (args) =>
        {
            Debug.Log("OnmatchInGameAccess : " + args.ErrInfo);
            ProcessMatchInGameAccess(args);
        };
        Backend.Match.OnMatchInGameStart += () =>
        {
            Debug.Log("게임시작");
            GameSetup();
        };
        Backend.Match.OnLeaveMatchMakingServer += (args) =>
        {
            Debug.Log("OnLeaveMatchMakingServer ");

        };
        Backend.Match.OnMatchResult += (args) =>
        {
            Debug.Log("게임 결과값 업로드 결과 : " + string.Format("{0} : {1}", args.ErrInfo, args.Reason));
            // 서버에서 게임 결과 패킷을 보내면 호출
            // 내가(클라이언트가) 서버로 보낸 결과값이 정상적으로 업데이트 되었는지 확인

            if (args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
              //  GameManager.GetInstance().ChangeState(GameManager.GameState.Result);
            }
            else if (args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_InGame_Timeout)
            {
                Debug.Log("게임 입장 실패 : " + args.ErrInfo);
              //  LobbyUI.GetInstance().MatchCancelCallback();
            }
            else
            {
                //InGameUiManager.instance.SetGameResult("결과 종합 실패\n호스트와 연결이 끊겼습니다.");
                Debug.Log("게임 결과 업로드 실패 : " + args.ErrInfo);
            }
            // 세션리스트 초기화
            SessionIdList = null;
        };
        Backend.Match.OnMatchRelay += (args) =>
        {
            if (PrevGameMessage(args.BinaryUserData) == true)
            {
                // 게임 사전 설정을 진행하였으면 바로 리턴
                return;
            }

            if (WorldManasger.instance == null)
            {
                // 월드 매니저가 존재하지 않으면 바로 리턴
                return;
            }

            WorldManasger.instance.OnRecieve(args);
        };
        Backend.Match.OnLeaveInGameServer += (args) =>
        {
            Debug.Log("OnLeaveInGameServer : " + args.ErrInfo + " : " + args.Reason);
            if (args.Reason.Equals("Fail To Reconnect"))
            {
                JoinMatchServer();
            }
            isConnectInGameServer = false;
        };

        Backend.Match.OnSessionOnline += (args) =>
        {
            // 다른 유저가 재접속 했을 때 호출
            var nickName = Backend.Match.GetNickNameBySessionId(args.GameRecord.m_sessionId);
            Debug.Log(string.Format("[{0}] 온라인되었습니다. - {1} : {2}", nickName, args.ErrInfo, args.Reason));
            //ProcessSessionOnline(args.GameRecord.m_sessionId, nickName);
        };
       
        Backend.Match.OnSessionOffline += (args) =>
        {
            // 다른 유저 혹은 자기자신이 접속이 끊어졌을 때 호출
            Debug.Log(string.Format("[{0}] 오프라인되었습니다. - {1} : {2}", args.GameRecord.m_nickname, args.ErrInfo, args.Reason));
            // 인증 오류가 아니면 오프라인 프로세스 실행
            if (args.ErrInfo != ErrorCode.AuthenticationFailed)
            {
                ProcessSessionOffline(args.GameRecord.m_sessionId);
            }
            else
            {
                // 잘못된 재접속 시도 시 인증오류가 발생
            }
        };
       

        Backend.Match.OnChangeSuperGamer += (args) =>
        {
            Debug.Log(string.Format("이전 방장 : {0} / 새 방장 : {1}", args.OldSuperUserRecord.m_nickname, args.NewSuperUserRecord.m_nickname));
            // 호스트 재설정
            SetSubHost(args.NewSuperUserRecord.m_sessionId);
            if (isHost)
            {
                // 만약 서브호스트로 설정되면 다른 모든 클라이언트에 싱크메시지 전송
                Invoke("SendGameSyncMessage", 1.0f);
            }
        };
    }
 
    public bool IsMySessionId(SessionId session)
    {
        return Backend.Match.GetMySessionId() == session;
    }
    private bool SetHostSession()
    {
        // 호스트 세션 정하기
        // 각 클라이언트가 모두 수행 (호스트 세션 정하는 로직은 모두 같으므로 각각의 클라이언트가 모두 로직을 수행하지만 결과값은 같다.)

        Debug.Log("호스트 세션 설정 진입");
        // 호스트 세션 정렬 (각 클라이언트마다 입장 순서가 다를 수 있기 때문에 정렬)
        SessionIdList.Sort();
        isHost = false;
        // 내가 호스트 세션인지
        foreach (var record in gameRecords)
        {
            if (record.Value.m_isSuperGamer == true)
            {
                if (record.Value.m_sessionId.Equals(Backend.Match.GetMySessionId()))
                {
                    isHost = true;
                }
                hostSession = record.Value.m_sessionId;
                break;
            }
        }

        Debug.Log("호스트 여부 : " + isHost);

        // 호스트 세션이면 로컬에서 처리하는 패킷이 있으므로 로컬 큐를 생성해준다
        if (isHost)
        {
            localQueue = new Queue<KeyMessage>();
        }
        else
        {
            localQueue = null;
        }

        // 호스트 설정까지 끝나면 매치서버와 접속 끊음
        return true;
    }
    public bool IsSessionListNull()
    {
        return SessionIdList == null || SessionIdList.Count == 0;
    }
    public string GetNickNameBySessionId(SessionId session)
    {
        // return Backend.Match.GetNickNameBySessionId(session);
        return gameRecords[session].m_nickname;
    }
    private void SetSubHost(SessionId hostSessionId)
    {
        Debug.Log("서브 호스트 세션 설정 진입");
        // 누가 서브 호스트 세션인지 서버에서 보낸 정보값 확인
        // 서버에서 보낸 SuperGamer 정보로 GameRecords의 SuperGamer 정보 갱신
        foreach (var record in gameRecords)
        {
            if (record.Value.m_sessionId.Equals(hostSessionId))
            {
                record.Value.m_isSuperGamer = true;
            }
            else
            {
                record.Value.m_isSuperGamer = false;
            }
        }
        // 내가 호스트 세션인지 확인
        if (hostSessionId.Equals(Backend.Match.GetMySessionId()))
        {
            isHost = true;
        }
        else
        {
            isHost = false;
        }

        hostSession = hostSessionId;

        Debug.Log("서브 호스트 여부 : " + isHost);
        // 호스트 세션이면 로컬에서 처리하는 패킷이 있으므로 로컬 큐를 생성해준다
        if (isHost)
        {
            localQueue = new Queue<KeyMessage>();
        }
        else
        {
            localQueue = null;
        }

        Debug.Log("서브 호스트 설정 완료");
    }
    public bool IsHost()
    {
        return isHost;
    }
    private void ProcessAccessMatchMakingServer(ErrorInfo errorinfo)
    {
        if (errorinfo != ErrorInfo.Success)
        {
            isConnectMatchServer = false;
        }
        if (!isConnectMatchServer)
        {
            Debug.Log("접속 실패");
        }
        else
        {
            Debug.Log("접속 성공");

        }
    }
    private void ExceptionHandler()
    {
        // 예외가 발생했을 때 호출
        Backend.Match.OnException += (e) =>
        {
            Debug.Log(e);   
        };
    }

    public void SetHostSession(SessionId host)
    {
        hostSession = host;
    }

    private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
    {
        ErrorInfo errorInfo;
        if (SessionIdList != null)
        {
            Debug.Log("이전 세션 저장 정보");
            SessionIdList.Clear();
        }
        if (!Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo))
        {
            Debug.Log("조잉서버");
        }
        isConnectMatchServer = true;
        isJoinGameRoom = false;

        isReconnectProcess = false;
        inGameRoomToken = args.RoomInfo.m_inGameRoomToken;
        isSandBoxGame = args.RoomInfo.m_enableSandbox;
        var info = GetMatchInfo(args.MatchCardIndate);
        if (info == null)
        {
            Debug.LogError("매치 정보를 불러오는 데 실패했습니다.");
            return;
        }

        nowMatchType = info.matchType;
        nowModeType = info.matchModeType;
        numOfClient = int.Parse(info.headCount);
    }
    public MatchInfo GetMatchInfo(string indate)
    {
        var result = matchInfos.FirstOrDefault(x => x.inDate == indate);
        if(result.Equals(default(MatchInfo))==true)
        {
            return null;
        }
        return result;
    }
}
public class ServerInfo
{
    public string host;
    public ushort port;
    public string roomToken;
}
public class MatchRecord
{
    public MatchType matchType;
    public MatchModeType modeType;
    public string matchTitle;
    public string score = "-";
    public int win = -1;
    public int numOfMatch = 0;
    public double winRate = 0;
}