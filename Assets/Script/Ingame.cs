using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Ingame : MonoBehaviour
{
    public Vector3 Fpos;
    public GameObject[] Effect;
    int ddddd = 1;
    int asdfiadsfdsfasd = 0;
    int[] vdfasdfas;
    public GameObject[] PlayeImg;
    public Transform[] SpawnPos1 = new Transform[3];
    public GameObject[] Card = new GameObject[3];
    public GameObject[] CheckTurn;
    public GameObject c;
    public GameObject cc;
    public GameObject ccc;
    public Quaternion[] f = new Quaternion[3];
    public GameObject cccc;
    private int a = 1;
    public GameObject Player;
    public GameObject[] Pos;
    public GameObject[] Dice;
    public GameObject ShieldUI;
    public GameObject Hp;
    public GameObject objd;
    int f2 = 0;
    // Start is called before the first frame update
    void Start()
    {
        Fpos = PlayeImg[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(UIManager.GetInstance().PlayerTurn + "PlayerT");
        Debug.Log(UIManager.GetInstance()._NextTurn + "NextT");
        Debug.Log(GameManager.GetInstance().NewTurn + "NewT");
        Debug.Log(UIManager.GetInstance().DieOn + "DieOn");


        if (UIManager.GetInstance()._NextTurn == true)
        {
            CheckTurn[0].SetActive(true);
        }
        else
        {
            CheckTurn[0].SetActive(false);

        }
        if (UIManager.GetInstance().PlayerTurn == true)
        {
            CheckTurn[1].SetActive(true);

        }
        else
        {
            CheckTurn[1].SetActive(false);
        }
        Dice[0] = GameObject.Find("D6_B (1)(Clone)");
        Dice[1] = GameObject.Find("D6_B(Clone)");
        Dice[2] = GameObject.Find("D6_B (2)(Clone)");

        cccc.GetComponent<Text>().text = Player.GetComponent<_Player>().hpEnemy.ToString();
        Hp.GetComponent<Text>().text = Player.GetComponent<_Player>().Hp.ToString();
        ShieldUI.GetComponent<Text>().text = Player.GetComponent<_Player>().Shield.ToString();
        if (f2 == 1)
        {
            if (SpawnPos1[2].GetChild(0) != null)
            {
                Protocol.EnemyCardMessage1 enemyCard = new Protocol.EnemyCardMessage1(Player.GetComponent<_Player>().GetIndex(), SpawnPos1[0].GetChild(0).GetComponent<Card>().a, SpawnPos1[1].GetChild(0).GetComponent<Card>().a, SpawnPos1[2].GetChild(0).GetComponent<Card>().a, ServerManager.GetInstance().myNickName);
                MatchManager.GetInstance().SendDataToInGame<Protocol.EnemyCardMessage1>(enemyCard);
                f2 = 0;
            }
        }


        if (Dice[0] != null && GameManager.GetInstance().NewTurn == true)
        {

            if ((Dice[0].GetComponent<Die>().AllStop == true) && (Dice[1].GetComponent<Die>().AllStop == true) && (Dice[2].GetComponent<Die>().AllStop == true))
            {
                Dice[0].GetComponent<Rigidbody>().freezeRotation = true;
                Dice[1].GetComponent<Rigidbody>().freezeRotation = true;
                Dice[2].GetComponent<Rigidbody>().freezeRotation = true;
                Dice[0].GetComponent<MeshCollider>().isTrigger = true;
                Dice[1].GetComponent<MeshCollider>().isTrigger = true;
                Dice[2].GetComponent<MeshCollider>().isTrigger = true;
                Dice[0].GetComponent<Rigidbody>().useGravity = false;
                Dice[1].GetComponent<Rigidbody>().useGravity = false;
                Dice[2].GetComponent<Rigidbody>().useGravity = false;


                f[0] = Dice[0].transform.rotation;
                f[1] = Dice[1].transform.rotation;
                f[2] = Dice[2].transform.rotation;

                Dice[0].transform.position = Pos[0].transform.position;
                Dice[0].transform.rotation = new Quaternion(f[0].x, f[0].y, 0, 0);
                Dice[0].GetComponent<Die>().firstpos = Dice[0].transform.position;
                Dice[1].transform.position = Pos[1].transform.position;
                Dice[1].transform.rotation = new Quaternion(f[1].x, f[1].y, 0, 0);
                Dice[1].GetComponent<Die>().firstpos = Dice[1].transform.position;


                Dice[2].transform.position = Pos[2].transform.position;
                Dice[2].transform.rotation = new Quaternion(f[2].x, f[2].y, 0, 0);
                Dice[2].GetComponent<Die>().firstpos = Dice[2].transform.position;





                GameManager.GetInstance().NewTurn = false;
                for (int i = 0; i < 3; i++)
                {
                }
                c = (GameObject)Instantiate(Card[0], SpawnPos1[0]);
                cc = (GameObject)Instantiate(Card[1], SpawnPos1[1]);
                ccc = (GameObject)Instantiate(Card[2], SpawnPos1[2]);


                f2 = 1;
            }
        }

        if (UIManager.GetInstance()._NextTurn == true && UIManager.GetInstance().PlayerTurn == true)
        {

            if (ddddd == 1)
            {
                c.GetComponent<Card>().ClickNext();
                foreach (var a in WorldManasger.instance.objlist)
                {
                    Destroy(a);
                }
            
                ddddd--;
            }

        }
        else
        {
            ddddd = 1;
        }
        if (asdfiadsfdsfasd == 0)
        {
            if (c != null)
            {
                if (c.GetComponent<Card>().END == true)
                {
                    cc.GetComponent<Card>().ClickNext();
                    asdfiadsfdsfasd = 1;
                }
            }
        }
        else if (asdfiadsfdsfasd == 1)
        {
            if (cc != null)
            {
                if (cc.GetComponent<Card>().END == true)
                {
                    ccc.GetComponent<Card>().ClickNext();
                    asdfiadsfdsfasd = 2;
                }
            }
        }
        else if (asdfiadsfdsfasd == 2)
            if (ccc != null)
            {
                Debug.Log("asbdsa2" + asdfiadsfdsfasd);
                Debug.Log("end2" + ccc.GetComponent<Card>().END);

                if (ccc.GetComponent<Card>().END == true)
                {
                    Debug.Log("asbdsa2"+asdfiadsfdsfasd);
                    Debug.Log("뺴애앰");
                    asdfiadsfdsfasd = 0;
                    GameManager.GetInstance().NextTurnReset();
                    UIManager.GetInstance().PlayerTurn = false;
                    UIManager.GetInstance()._NextTurn = false;
                    GameManager.GetInstance().NewTurn = true;
                    UIManager.GetInstance().DieOn = false;
                    Dice[2].GetComponent<Die>().DelDice();
                    Dice[1].GetComponent<Die>().DelDice();
                    Dice[0].GetComponent<Die>().DelDice();
                    Destroy(c);
                    Destroy(cc);
                    Destroy(ccc);
                }
            }

    }
            
}




