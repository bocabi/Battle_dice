using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public bool PlayerTurn = false;
    public GameObject[] Dice;
    private GameObject[] card;
    public GameObject[] SpawnPosision;
    int[] a = new int[3];
    public Text[] text = new Text[3];
    public GameObject Player;
    public bool DieOn = false;
    public bool _NextTurn = false;
    public bool STOP;
    public bool Next = false;
    public int EnemyHp;
    private static UIManager instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(this.gameObject);
    }
    public static UIManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("BackEndServerManager 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }



    public void SpawnD()
    {
        if (DieOn == false)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(Dice[i], SpawnPosision[i].transform);
            }
            DieOn = true;
        }
    }

    public void StopClick()
    {
        STOP = true;
    }
    public void NextTurn()
    {

        if (GameManager.GetInstance().NewTurn == false)
        {
            Protocol.NextTurnMessage message = new Protocol.NextTurnMessage(Player.GetComponent<_Player>().GetIndex(), true, ServerManager.GetInstance().myNickName);
            MatchManager.GetInstance().SendDataToInGame<Protocol.NextTurnMessage>(message);
            _NextTurn = true;
        }


    }
    public void SendATK()
    {
        Protocol.ATKMessage message = new Protocol.ATKMessage(WorldManasger.instance.GetMyPlayerIndex(), GameManager.GetInstance().Dmg, GameManager.GetInstance().DmgPlus, GameManager.GetInstance().MgDmg, ServerManager.GetInstance().myNickName);
        MatchManager.GetInstance().SendDataToInGame<Protocol.ATKMessage>(message);
    }
    IEnumerator Nextgogo()
    {
        yield return new WaitForSeconds(0.01f);
        Next = false;

        yield return new WaitForSeconds(0.1f);
        GameManager.GetInstance().NewTurn = true;
    }
}
