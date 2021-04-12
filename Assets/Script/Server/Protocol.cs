using BackEnd.Tcp;
using UnityEngine;
using System.Collections.Generic;

namespace Protocol
{
    // 이벤트 타입
    public enum Type : sbyte
    {
        Key = 0,        // 키(가상 조이스틱) 입력
        NextTurn,
        Hp_,
        EnemyCard1,
        EnemyCard2,
        EnemyCard,
        PlusShield,
        AIPlayerInfo,
        ATK,// AI가 존재하는 경우 AI 정보
        LoadRoomScene,      // 룸 씬으로 전환
        LoadGameScene,      // 인게임 씬으로 전환
        StartCount,     // 시작 카운트
        GameStart,      // 게임 시작
        GameEnd,        // 게임 종료
        GameSync,       // 플레이어 재접속 시 게임 현재 상황 싱크
        Max
    }

    // 애니메이션 싱크는 사용하지 않습니다.
    /*
    public enum AnimIndex
    {
        idle = 0,
        walk,
        walkBack,
        stop,
        max
    }
    */

    public class PlusShield : Message
    {
        public  int Shield;
        public SessionId playersession;
        public string NickName;
        public PlusShield(int Shield, string NickName, SessionId playersession) : base(Type.PlusShield)
        {
            this.Shield = Shield;
            this.playersession = playersession;
            this.NickName = NickName;
        }
    }

    // 조이스틱 키 이벤트 코드
    public static class KeyEventCode
    {
        public const int NONE = 0;
        public const int MOVE = 1;      // 이동 메시지
        public const int ATTACK = 2;    // 공격 메시지
        public const int NO_MOVE = 4;   // 이동 멈춤 메시지
    }

    public class NextTurnMessage : Message
    {
        public SessionId playerSession;
        public bool NextTurn = false;
        public string NickName;
        public NextTurnMessage(SessionId session,bool Turn, string NickName) : base(Type.NextTurn)
        {
            this.playerSession = session;
            this.NextTurn = Turn;
            this.NickName = NickName;
        }
    }
    
    public class ATKMessage : Message
    {
        public SessionId playerSession;
        public int dmg,dmgPlus,mgDmg;
        public bool ishost;
        public string NickName;

        public ATKMessage(SessionId session, int dmg, int dmgPlus, int mgDmg, string NickName) : base(Type.ATK)
        {
            this.playerSession = session;
            this.dmg = dmg;
            this.dmgPlus = dmgPlus;
            this.mgDmg = mgDmg;
            this.NickName = NickName;

        }
    }
    public class NowHp : Message
    {
        public SessionId PlayerSession;
        public int Hp;
        public string NickName;
        public NowHp(SessionId PlayerSession,int hp, string NickName) : base(Type.Hp_)
        {
            this.PlayerSession = PlayerSession;
            this.Hp = hp;
        }
    }
   

        
    public class EnemyCardMessage1 : Message
    {
        public SessionId playerSession;
        public int CardNum1;
        public int CardNum2;
        public int CardNum3;

        public string NickName;
        public EnemyCardMessage1(SessionId playerSession, int CardNum1, int CardNum2, int CardNum3, string NickName) : base(Type.EnemyCard)
        {
            this.playerSession = playerSession;
            this.CardNum1 = CardNum1;
            this.CardNum2 = CardNum2;
            this.CardNum3 = CardNum3;

            this.NickName = NickName;
        }
    }
   
 
    public class Message
    {
        public Type type;

        public Message(Type type)
        {
            this.type = type;
        }
    }

    public class KeyMessage : Message
    {
        public int keyData;
        public float x;
        public float y;
        public float z;

        public KeyMessage(int data, Vector3 pos) : base(Type.Key)
        {
            this.keyData = data;
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }
    }


    public class GameEndMessage : Message
    {
        public int Mcount;
        public int[] SessionList;
        public GameEndMessage(Stack<SessionId> result) : base(Type.GameEnd)
        {
            Mcount = result.Count;
            SessionList = new int[Mcount];
            for (int i = 0; i < Mcount; ++i)
            {
                SessionList[i] = (int)result.Pop();
            }
        }
    }




    public class AIPlayerInfo : Message
    {
        public SessionId m_sessionId;
        public string m_nickname;
        public byte m_teamNumber;
        public int m_numberOfMatches;
        public int m_numberOfWin;
        public int m_numberOfDraw;
        public int m_numberOfDefeats;
        public int m_points;
        public int m_mmr;

        public AIPlayerInfo(MatchUserGameRecord gameRecord) : base(Type.AIPlayerInfo)
        {
            this.m_sessionId = gameRecord.m_sessionId;
            this.m_nickname = gameRecord.m_nickname;
            this.m_teamNumber = gameRecord.m_teamNumber;
            this.m_numberOfWin = gameRecord.m_numberOfWin;
            this.m_numberOfDraw = gameRecord.m_numberOfDraw;
            this.m_numberOfDefeats = gameRecord.m_numberOfDefeats;
            this.m_points = gameRecord.m_points;
            this.m_mmr = gameRecord.m_mmr;
            this.m_numberOfMatches = gameRecord.m_numberOfMatches;
        }

        public MatchUserGameRecord GetMatchRecord()
        {
            MatchUserGameRecord gameRecord = new MatchUserGameRecord();
            gameRecord.m_sessionId = this.m_sessionId;
            gameRecord.m_nickname = this.m_nickname;
            gameRecord.m_numberOfMatches = this.m_numberOfMatches;
            gameRecord.m_numberOfWin = this.m_numberOfWin;
            gameRecord.m_numberOfDraw = this.m_numberOfDraw;
            gameRecord.m_numberOfDefeats = this.m_numberOfDefeats;
            gameRecord.m_mmr = this.m_mmr;
            gameRecord.m_points = this.m_points;
            gameRecord.m_teamNumber = this.m_teamNumber;

            return gameRecord;
        }
    }

    public class LoadRoomSceneMessage : Message
    {
        public LoadRoomSceneMessage() : base(Type.LoadRoomScene)
        {

        }
    }

    public class LoadGameSceneMessage : Message
    {
        public LoadGameSceneMessage() : base(Type.LoadGameScene)
        {

        }
    }

    public class StartCountMessage : Message
    {
        public int time;
        public StartCountMessage(int time) : base(Type.StartCount)
        {
            this.time = time;
        }
    }

    public class GameStartMessage : Message
    {
        public GameStartMessage() : base(Type.GameStart) { }
    }



    public class GameSyncMessage : Message
    {
        public SessionId host;
        public int count = 0;
        public float[] xPos = null;
        public float[] zPos = null;
        public int[] hpValue = null;
        public bool[] onlineInfo = null;

        public GameSyncMessage(SessionId host, int count, float[] x, float[] z, int[] hp, bool[] online) : base(Type.GameSync)
        {
            this.host = host;
            this.count = count;
            this.xPos = new float[count];
            this.zPos = new float[count];
            this.hpValue = new int[count];
            this.onlineInfo = new bool[count];

            for (int i = 0; i < count; ++i)
            {
                xPos[i] = x[i];
                zPos[i] = z[i];
                hpValue[i] = hp[i];
                onlineInfo[i] = online[i];
            }
        }
    }
}
