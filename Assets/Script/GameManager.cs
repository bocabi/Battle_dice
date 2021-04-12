using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using BackEnd.Tcp;
using BackEnd;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private SessionId index = 0;
    public static event Action OnGameOver = delegate { };

    int MYHP;
    public int Dmg;
    public int DmgPlus;
    public int MgDmg;
    public int Armor;
    public int Heal;
    public bool Miss;
    public int Ice, Fire, Eternal;
    public bool ArmorBreak;
    public int DelHp;
    public int DelMyHp;
    public struct HeroDeck
    {
        public List<int[]> DeckKind;
    }
    enum Hero
    {
        Warrior,
        Rogue,
        Shaman
    }
    
    private GameObject obb;
    private GameObject obb1;
    private GameObject obb2;
    public bool NewTurn = true;
    public bool tpoi = false;
    private int card;
    private HeroDeck heroDeck;
    public int[] MyDeck = new int[8];
    private int Deck;
    public int[] NowDeck = new int[8];
    public HeroDeck[] HeroKind = new HeroDeck[3];
    private static GameManager instance;
    private int HeroNum = 0;

    void Awake()
    {
        Screen.SetResolution(1080, 1920, true);

        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(this.gameObject);
    }
    public void GameOver()
    {
        OnGameOver();
    }
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    void Start()
    {

        MYHP = 30;
        obb = new GameObject();
        if (HeroKind[0].DeckKind == null)
        {
            HeroKind[0].DeckKind = new List<int[]>();
            HeroKind[0].DeckKind.Add(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        }

        MyDeck = HeroKind[0].DeckKind[0];
    }

    private void Update()
    {

      

        HeroKind[HeroNum].DeckKind[Deck] = NowDeck;
    }
    public void HeroSeletion(int a)
    {
        HeroNum = a;
    }
    public void DeckSeletion(int a)
    {
        Deck = a;
    }
    public void CardSeletion(int a)
    {
        card = a;
        Debug.Log(NowDeck[a]);
        NowDeck[0] = card;

    }
   
    public void PlusA(int dmg, int dmgPlus, int mgdmg, int armor, int heal, bool miss, int ice, int fire, int eternal, int delHp, int delMyhp)
    {
        Dmg += dmg;
        DmgPlus += dmgPlus;
        MgDmg += MgDmg;
        Armor += armor;
        MYHP += heal;
        Miss = miss;
        Ice += ice;
        Fire += fire;
        Eternal = eternal;
        //상대 HP - delHp;
        MYHP -= delMyhp;
    }
    public void BreakArmor()
    {
        Armor = 0;
    }
   
    public void NextTurnReset()
    {
        Dmg = 0;
        DmgPlus = 0;
        MgDmg = 0;
        Miss = false;
        Ice--;
        Fire--;

    }

}
