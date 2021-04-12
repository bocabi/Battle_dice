using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;
public partial class MatchManager : MonoBehaviour
{
    private bool isSetHost = false;
    private string NUM_INGAME_SESSION = "인게임 내 세션 갯수 : {0}";
    MatchGameResult matchGameResult;
    //private MatchGameReult
    // Start is called before the first frame update
    private void ProcessSessionOffline(SessionId sessionId)
    {
        if (hostSession.Equals(sessionId))
        {
            // 호스트 연결 대기를 띄움
            //InGameUiManager.GetInstance().SetHostWaitBoard();
        }
        else
        {
            // 호스트가 아니면 단순히 UI 만 띄운다.
        }
    }
    public bool PrevGameMessage(byte[] BinaryUserData)
    {
        Protocol.Message msg = DataParser.ReadJsonData<Protocol.Message>(BinaryUserData);
        if (msg == null)
        {
            return false;
        }

        // 게임 설정 사전 작업 패킷 검사 
        switch (msg.type)
        {
            case Protocol.Type.AIPlayerInfo:
                Protocol.AIPlayerInfo aiPlayerInfo = DataParser.ReadJsonData<Protocol.AIPlayerInfo>(BinaryUserData);
                //ProcessAIDate(aiPlayerInfo);
                return true;
            case Protocol.Type.LoadRoomScene:
                //LobbyUI.GetInstance().ChangeRoomLoadScene();
                if (IsHost() == true)
                {
                    Debug.Log("5초 후 게임 씬 전환 메시지 송신");
                    Invoke("SendChangeGameScene", 5f);
                }
                return true;
            case Protocol.Type.LoadGameScene:
                //GameManager.GetInstance().ChangeState(GameManager.GameState.Start);
                return true;
        }
        return false;
    }
    private void ProcessMatchInGameSessionList(MatchInGameSessionListEventArgs args)
    {
        SessionIdList = new List<SessionId>();
        gameRecords = new Dictionary<SessionId, MatchUserGameRecord>();

        foreach (var record in args.GameRecords)
        {
            SessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);
        }
        SessionIdList.Sort();
    }
  
    private void ProcessMatchInGameAccess(MatchInGameSessionEventArgs args)
    {
        if (isReconnectProcess)
        {
            Debug.Log("재접속 프로세스 진행중...");
            return;
        }
        Debug.Log("썽공");
        if (args.ErrInfo != ErrorCode.Success)
        {
            LeaveInGameRoom();
            return;
        }
        var record = args.GameRecord;
        Debug.Log(MatchManager.GetInstance().IsHost());
        Debug.Log("인게임 접속 성공");
        Debug.Log(string.Format(string.Format("인게임 접속 유저 정보 [{0}] : {1}", args.GameRecord.m_sessionId, args.GameRecord.m_nickname)));

        if (!SessionIdList.Contains(args.GameRecord.m_sessionId))
        {
            SessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);
            Debug.Log(string.Format(NUM_INGAME_SESSION, SessionIdList.Count));
        }
    }
    public void SendDataToInGame<T>(T msg)
    {
        var byteArray = DataParser.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }
  
    public void OnGameReady()
    {
        if (isSetHost == false)
        {
            // 호스트가 설정되지 않은 상태이면 호스트 설정
            isSetHost = SetHostSession();
        }
        Debug.Log("호스트 설정 완료");

        //if (isSandBoxGame == true && IsHost() == true)
        //{
        //    SetAIPlayer();
        //}

        //if (IsHost() == true)
        //{
        //    // 0.5초 후 ReadyToLoadRoom 함수 호출
        //    Invoke("ReadyToLoadRoom", 0.5f);
        //}
        SceneManager.LoadScene("GameScene");
    }
    public void SetPlayerSessionList(List<SessionId> sessions)
    {
        SessionIdList = sessions;
    }
    private void GameSetup()
    {
        Debug.Log("게임 시작 메시지 수신. 게임 설정 시작");
        // 게임 시작 메시지가 오면 게임을 레디 상태로 변경
       
            isHost = false;
            isSetHost = false;
            OnGameReady();
          
    }
    private void AccessInGameRoom(string roomToken)
    {
        Backend.Match.JoinGameRoom(roomToken);
    }

    private MatchGameResult OneOnOneRecord(Stack<SessionId> record)
    {
        MatchGameResult nowGameResult = new MatchGameResult();
        foreach(var sessionId in record)
        {
        }
        nowGameResult.m_winners = new List<SessionId>();
        nowGameResult.m_losers = new List<SessionId>();

        nowGameResult.m_winners.Add(record.Pop());
        nowGameResult.m_losers.Add(record.Pop());

        Debug.Log("쿠쿠루삥뽕");
        nowGameResult.m_draws = null;

        return nowGameResult;
    }
    private void RemoveAISessionInGameResult()
    {
        string str = string.Empty;
        List<SessionId> aiSession = new List<SessionId>();
        if (matchGameResult.m_winners != null)
        {
            str += "승자 : ";
            foreach (var tmp in matchGameResult.m_winners)
            {
                if ((int)tmp < (int)SessionId.Reserve)
                {
                    aiSession.Add(tmp);
                }
                else
                {
                    str += tmp + " : ";
                }
            }
            str += "\n";
            matchGameResult.m_winners.RemoveAll(aiSession.Contains);
        }

        aiSession.Clear();
        if (matchGameResult.m_losers != null)
        {
            str += "패자 : ";
            foreach (var tmp in matchGameResult.m_losers)
            {
                if ((int)tmp < (int)SessionId.Reserve)
                {
                    aiSession.Add(tmp);
                }
                else
                {
                    str += tmp + " : ";
                }
            }
            str += "\n";
            matchGameResult.m_losers.RemoveAll(aiSession.Contains);
        }
        Debug.Log(str);
    }
    public void SetGameResult(MatchGameResult matchGameResult)
    {
        var matchInstance = MatchManager.GetInstance();
        if (matchInstance == null)
        {   
            return;
        }
        if (matchInstance.nowModeType != MatchModeType.Melee)
        {
            string winData ;
            string loseData;
            

            string winner = "";
            string loser = "";
            foreach (var user in matchGameResult.m_winners)
            {
                winner += matchInstance.GetNickNameBySessionId(user) + "\n";
            }

            foreach (var user in matchGameResult.m_losers)
            {
                loser += matchInstance.GetNickNameBySessionId(user) + "\n";
            }

            winData = winner;
            loseData = loser;
            Debug.Log(winner + ServerManager.GetInstance().myNickName);
        }
    }
    public void MatchGameOver(Stack<SessionId>record)
    {
      
        if(nowModeType == MatchModeType.OneOnOne)
        {
            matchGameResult = OneOnOneRecord(record);
            Debug.Log("OneOnOne");
        }

        SetGameResult(matchGameResult);
        RemoveAISessionInGameResult();
        Backend.Match.MatchEnd(matchGameResult);

    }
    // 인게임 서버 접속 종료
    public void LeaveInGameRoom()
    {
        isConnectInGameServer = false;
        Backend.Match.LeaveGameServer();
        Debug.Log("인게임 서버 접속 종료");
    }

}
