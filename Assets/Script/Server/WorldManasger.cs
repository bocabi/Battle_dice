using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using BackEnd;
using BackEnd.Tcp;

public class WorldManasger : MonoBehaviour
{
    public List<GameObject> objlist;
    public Stack<SessionId> gameRecord;
    public GameObject[] dfas;
    public GameObject[] EnemyCardPosision;
    public GameObject player_;
    public int alivePlayer { get; set; }
    public GameObject EnemyCard;
    static public WorldManasger instance;
    public Dictionary<SessionId, _Player> players;
    private SessionId myPlayerIndex = SessionId.None;
    public delegate void PlayerDie(SessionId index);
    public PlayerDie dieEvent;
    private void Awake()
    {
        instance = this;
    }
    public GameObject playerPool;
    //  private Dictionary<SessionId, Player> players;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGame();
        var matchInstance = MatchManager.GetInstance();
        if (matchInstance == null)
        {
            return;
        }
        if (matchInstance.isReconnectProcess)
        {
            //InGameUiManager.GetInstance().SetStartCount(0, false);
            //InGameUiManager.GetInstance().SetReconnectBoard(BackEndServerManager.GetInstance().myNickName);
        }
    }
    //public SessionId GetMyPlayerIndex()
    //{
    //    return myPlayerIndex;
    //}

    public void ExitGame()
    {

    }
    private void ProcessSyncData(GameSyncMessage syncMessage)
    {
        // 플레이어 데이터 동기화
        int index = 0;
        //if (players == null)
        //{
        //    Debug.LogError("Player Poll is null!");
        //    return;
        //}
        //foreach (var player in players)
        //{
        //    var y = player.Value.GetPosition().y;
        //    player.Value.SetPosition(new Vector3(syncMessage.xPos[index], y, syncMessage.zPos[index]));
        //    player.Value.SetHP(syncMessage.hpValue[index]);
        //    index++;
        //}
        MatchManager.GetInstance().SetHostSession(syncMessage.host);
    }
    private void PlayerDieEvent(SessionId index)
    {
        alivePlayer -= 1;
        Debug.Log(MatchManager.GetInstance().IsHost());
        players[index].gameObject.SetActive(false);
        Debug.Log(string.Format("Player Die : " + players[index].GetNickName()));
        gameRecord.Push(index);
        if (!MatchManager.GetInstance().IsHost())
        {
            return;
        }
        foreach (var t in MatchManager.GetInstance().SessionIdList)
        {
            if(players[t].isLive1 == true)
            {
                gameRecord.Push(t);
            }
        }
        if (alivePlayer <= 1)
        {
            Debug.Log("최민수돼지");
            SendGameEndOrder();
        }
        
    }
    public void _ExitGame()
    {


        players[myPlayerIndex].GetComponent<_Player>().Hp = 0;
        if (players[myPlayerIndex].GetComponent<_Player>().Hp <= 0)
        {
            Debug.Log("gd");
            players[myPlayerIndex].GetComponent<_Player>().isLive1 = false;
        }
        
        Protocol.NowHp messageh = new Protocol.NowHp(myPlayerIndex, players[myPlayerIndex].GetComponent<_Player>().Hp, player_.GetComponent<_Player>().GetNickName());
        MatchManager.GetInstance().SendDataToInGame<Protocol.NowHp>(messageh);

    }
    private void SendGameEndOrder()
    {
        // 게임 종료 전환 메시지는 호스트에서만 보냄
        Debug.Log("Make GameResult & Send Game End Order");
        //foreach (SessionId session in MatchManager.GetInstance().SessionIdList)
        //{
        //    Debug.Log(players[session].GetComponent<_Player>().GetNickName());
        //    Debug.Log(players[session].GetComponent<_Player>().GetNickName());

        //    if (players[session].GetIsLive() && !gameRecord.Contains(session))
        //    {
        //        gameRecord.Push(session);
        //    }
        //}
        foreach (var se in gameRecord)
        {
            Debug.Log("맻번?");
        }
        GameEndMessage message = new GameEndMessage(gameRecord);
        MatchManager.GetInstance().SendDataToInGame<GameEndMessage>(message);
    }

    public void OnGameStart()
    {

        if (MatchManager.GetInstance().IsHost())
        {
            Debug.Log("플레이어 세션정보 확인");
            if (MatchManager.GetInstance().IsSessionListNull())
            {
                Debug.Log("Player Index Not Exist");
                foreach (var session in MatchManager.GetInstance().SessionIdList)
                {
                    gameRecord.Push(session);
                }
                GameEndMessage gameEndMessage = new GameEndMessage(gameRecord);
                MatchManager.GetInstance().SendDataToInGame<GameEndMessage>(gameEndMessage);
                return;
            }
        }
        SetPlayerInfo();
        Protocol.NowHp messageh = new Protocol.NowHp(UIManager.GetInstance().Player.GetComponent<_Player>().GetIndex(), UIManager.GetInstance().Player.GetComponent<_Player>().Hp, player_.GetComponent<_Player>().GetNickName());
        MatchManager.GetInstance().SendDataToInGame<Protocol.NowHp>(messageh);
    }
    public bool InitializeGame()
    {
        objlist.Clear();
        GameManager.OnGameOver += delegate { };

        Debug.Log("게임 초기화 진행");
        gameRecord = new Stack<SessionId>();

        GameManager.OnGameOver += GameOver;


        dieEvent += PlayerDieEvent;
        OnGameStart();

        return true;
    }
    public SessionId GetMyPlayerIndex()
    {
        return myPlayerIndex;
    }
    // Update is called once per frame
    int i = 1;
    void Update()
    {


        if (i == 1)
            foreach(var session in MatchManager.GetInstance().SessionIdList)
            {
                if(players[session].isLive1 ==false)
                {
                    WorldManasger.instance.dieEvent(session);
                     i--;
                }
            }

    }
    public void SetPlayerInfo()
    {
        if (MatchManager.GetInstance().SessionIdList == null)
        {
            SetPlayerInfo();
            return;
        }
        var gamers = MatchManager.GetInstance().SessionIdList;
        int size = gamers.Count;
        if (size <= 0)
        {
            Debug.Log("No Player Exist");
            return;
        }
        if (size > 2)
        {
            Debug.Log("Player Pool Exceed");
            return;
        }
        players = new Dictionary<SessionId, _Player>();
        MatchManager.GetInstance().SetPlayerSessionList(gamers);
        int index = 0;
        foreach (var sessionId in gamers)
        {
            GameObject obj = Instantiate(player_, new Vector3(0, 0, 0), Quaternion.identity);
            players.Add(sessionId, player_.GetComponent<_Player>());
            if (MatchManager.GetInstance().IsMySessionId(sessionId))
            {
                myPlayerIndex = sessionId;
                players[sessionId].Initialize(true, myPlayerIndex, MatchManager.GetInstance().GetNickNameBySessionId(sessionId));

            }
            else
            {
                players[sessionId].Initialize(true, sessionId, MatchManager.GetInstance().GetNickNameBySessionId(sessionId));
            }
            index += 1;
        }
        alivePlayer = size;
    }
    public void GameOver()
    {
        Debug.Log("게임종료");
        if (MatchManager.GetInstance() == null)
        {
            Debug.Log("매치매니저널임");
            return;
        }
        MatchManager.GetInstance().MatchGameOver(gameRecord);
    }
    private void SeteGameRecord(int count, int[] arr)
    {
        gameRecord = new Stack<SessionId>();
        for (int i = count - 1; i >= 0; --i)
        {
            gameRecord.Push((SessionId)arr[i]);
        }
    }
    public void OnRecieve(MatchRelayEventArgs args)
    {
        if (args.BinaryUserData == null)
        {
            Debug.LogWarning(string.Format("빈 데이터가 브로드캐스팅 되었습니다.\n{0} - {1}", args.From, args.ErrInfo));
            // 데이터가 없으면 그냥 리턴
            return;
        }
        Message msg = DataParser.ReadJsonData<Message>(args.BinaryUserData);
        if (msg == null)
        {
            return;
        }
        if (MatchManager.GetInstance().IsHost() != true && args.From.SessionId == myPlayerIndex)
        {
            return;
        }
        if (players == null)
        {
            Debug.LogError("Players 정보가 존재하지 않습니다.");
            return;
        }
        switch (msg.type)
        {
            case Protocol.Type.StartCount:
                StartCountMessage startCount = DataParser.ReadJsonData<StartCountMessage>(args.BinaryUserData);
                Debug.Log("wait second : " + (startCount.time));
                // InGameUiManager.GetInstance().SetStartCount(startCount.time);
                break;
            case Protocol.Type.GameStart:
                // InGameUiManager.GetInstance().SetStartCount(0, false);
                //    GameManager.GetInstance().ChangeState(GameManager.GameState.InGame);
                break;
            case Protocol.Type.GameEnd:
                GameEndMessage endMessage = DataParser.ReadJsonData<GameEndMessage>(args.BinaryUserData);
                SeteGameRecord(endMessage.Mcount, endMessage.SessionList);
                GameManager.GetInstance().GameOver();
                break;

            case Protocol.Type.Key:
                KeyMessage keyMessage = DataParser.ReadJsonData<KeyMessage>(args.BinaryUserData);
                //   ProcessKeyEvent(args.From.SessionId, keyMessage);
                break;
            case Protocol.Type.NextTurn:
                NextTurnMessage message = DataParser.ReadJsonData<NextTurnMessage>(args.BinaryUserData);

                if (ServerManager.GetInstance().myNickName != message.NickName)
                {
                    UIManager.GetInstance().PlayerTurn = message.NextTurn;
                }
                break;
            case Protocol.Type.ATK:
                ATKMessage ATmessage = DataParser.ReadJsonData<ATKMessage>(args.BinaryUserData);

                if (ATmessage.playerSession != WorldManasger.instance.myPlayerIndex)
                {
                    int a = ATmessage.dmg + ATmessage.dmgPlus + ATmessage.mgDmg;
                    players[ATmessage.playerSession].GetComponent<_Player>().Hp -= a;

                    Protocol.NowHp messageh = new Protocol.NowHp(ATmessage.playerSession, players[ATmessage.playerSession].GetComponent<_Player>().Hp, players[ATmessage.playerSession].GetComponent<_Player>().GetNickName());
                    MatchManager.GetInstance().SendDataToInGame<Protocol.NowHp>(messageh);
                }
                break;
            case Protocol.Type.Hp_:
                {

                    NowHp hpMessage = DataParser.ReadJsonData<NowHp>(args.BinaryUserData);

                    if (hpMessage.PlayerSession != WorldManasger.instance.myPlayerIndex)
                    {
                        players[hpMessage.PlayerSession].GetComponent<_Player>().hpEnemy = hpMessage.Hp;
                    }
                }
                break;
            case Protocol.Type.EnemyCard:
                EnemyCardMessage1 message1 = DataParser.ReadJsonData<EnemyCardMessage1>(args.BinaryUserData);

                if (message1.NickName != ServerManager.GetInstance().myNickName)
                {
                    
                      Instantiate(EnemyCard, EnemyCardPosision[0].transform).GetComponent<EnemyCard>().a = message1.CardNum1;
                      Instantiate(EnemyCard, EnemyCardPosision[1].transform).GetComponent<EnemyCard>().a = message1.CardNum2;
                      Instantiate(EnemyCard, EnemyCardPosision[2].transform).GetComponent<EnemyCard>().a = message1.CardNum3;
                }

                break;
            // case Protocol.Type.PlayerMove:
            //  PlayerMoveMessage moveMessage = DataParser.ReadJsonData<PlayerMoveMessage>(args.BinaryUserData);
            //  ProcessPlayerData(moveMessage);
            //  break;
            // case Protocol.Type.PlayerAttack:
            //      PlayerAttackMessage attackMessage = DataParser.ReadJsonData<PlayerAttackMessage>(args.BinaryUserData);
            //      ProcessPlayerData(attackMessage);
            //     break;
            // case Protocol.Type.PlayerDamaged:
            //     PlayerDamegedMessage damegedMessage = DataParser.ReadJsonData<PlayerDamegedMessage>(args.BinaryUserData);
            //       ProcessPlayerData(damegedMessage);
            //   break;
            //  case Protocol.Type.PlayerNoMove:
            //      PlayerNoMoveMessage noMoveMessage = DataParser.ReadJsonData<PlayerNoMoveMessage>(args.BinaryUserData);
            //      ProcessPlayerData(noMoveMessage);
            //     break;
            case Protocol.Type.GameSync:
                GameSyncMessage syncMessage = DataParser.ReadJsonData<GameSyncMessage>(args.BinaryUserData);
                ProcessSyncData(syncMessage);
                break;
            default:
                Debug.Log("Unknown protocol type");
                return;
        }
    }
}
