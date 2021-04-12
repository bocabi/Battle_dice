using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] sprites = new Sprite[20];
    public bool END;
    public GameObject CARD;
    static public Card instance;
    public Transform Fpos;
    public GameObject EnemyPlayer;
    public GameObject TextObj;
    private int Dmg;
    public GameObject CARDMagEffect;
    public Animation animator;
    private int DmgPlus;
    private int MgDmg;
    private int Armor;
    private int Heal;
    private bool Miss;
    private int Ice, Fire, Eternal;
    private bool ArmorBreak;
    private int DelHp;
    private int DelMyHp;
    private Die GetDie;
    public int a = 1;
    public GameObject g;
    private string need;
    public GameObject Playera;
    public string d;
    public GameObject LerpPosision;
    public Vector3 nowpos;
    void Start()
    {
        EnemyPlayer = GameObject.Find("Desk_Hero_Backframe (1)");
        nowpos = this.transform.position;
        LerpPosision = GameObject.Find(d);
        a = GameManager.GetInstance().MyDeck[Random.Range(0, 8)];
        needdic();
        Instantiate(TextObj, this.transform.GetChild(3)).GetComponent<Text>().text = need;
        transform.GetComponent<SpriteRenderer>().sprite = sprites[a - 1];

        StartCoroutine(CardDraw());
    }

    private IEnumerator CardDraw()
    {

        for (int i = 0; i < 40; i++)
        {
            this.transform.position = Vector3.Lerp(nowpos, LerpPosision.transform.position, 0.025f * i);
            yield return new WaitForSeconds(.005f);

        }
    }
    void needdic()
    {
        switch (a)
        {
            case 1:
                need = "Free";
                break;
            case 2:
                need = "Free";
                break;
            case 3:
                need = "n < 3";
                break;
            case 4:
                need = "Free";

                break;
            case 5:
                need = "Free";

                break;
            case 6:
                need = "Free";

                break;
            case 7:
                need = "Free";

                break;
            case 8:
                need = "Free";

                break;
            case 9:
                need = "Free";

                break;
            case 10:
                need = "n == 6";
                break;
            case 11:

                break;
            case 12:
                need = "n <= 4";
                break;
            case 13:
                need = "n >= 4";
                break;
            case 14:
                need = "n >= 4";
                break;
            case 15:
                need = "n <= 4";
                break;
            case 16:
                need = "n == 6";
                break;
            case 17:
                need = "n == 6";
                break;
            case 18:
                need = "n == 6";
                break;

        }
    }
    //                                      if(Dice.GetComponent<Die>().MyEye == )
    // Update is called once per frame
    void Update()
    {


        if (transform.GetChild(2).childCount != 0)
            GetDie = transform.GetChild(2).GetChild(0).GetComponent<Die>();
    }
    //1티어
    public void ClickUI()
    {

    }
    public void ClickNext()
    {
        if (transform.GetChild(2).childCount != 0)
        {
            switch (a)
            {
                case 1:
                    {
                        Strike(GetDie.MyEye);
                        StartCoroutine("OnEffect");
                    }
                    break;
                case 2:
                    Defend(GetDie.MyEye);
                    END = true;
                    break;
                case 3:
                    if (GetDie.MyEye <= 3)
                    {
                        Bash(GetDie.MyEye);
                        StartCoroutine("OnEffect");
                    }
                    break;
                case 4:
                    Simmering_Anger(GetDie.MyEye);
                    END = true;
                    break;
                case 5:
                    Flame(GetDie.MyEye);
                    StartCoroutine("OnMgEffect");
                    break;
                case 6:
                    Increased_Defend(GetDie.MyEye);
                    END = true;

                    break;
                case 7:
                    Home_Thrust(GetDie.MyEye);
                    StartCoroutine("OnEffect");
                    break;
                case 8:
                    FireBall(GetDie.MyEye);
                    StartCoroutine("OnMgEffect");
                    break;
                case 9:
                    Arrows(GetDie.MyEye);
                    StartCoroutine("OnEffect");
                    break;
                case 10:
                    if (GetDie.MyEye == 6)
                        Hammer(GetDie.MyEye);
                    StartCoroutine("OnEffect");

                    break;
                case 11:
                    HpHeal(GetDie.MyEye);
                    Destroy(gameObject);
                    break;
                case 12:
                    if (GetDie.MyEye <= 4)
                    {
                        Blade_Storm(GetDie.MyEye);
                        StartCoroutine("OnEffect");

                    }
                    break;
                case 13:
                    if (GetDie.MyEye >= 4)
                    {
                        Body_Slam(GetDie.MyEye);
                        StartCoroutine("OnEffect");

                    }
                    break;
                case 14:
                    if (GetDie.MyEye >= 4)
                    {
                        Blizzard(GetDie.MyEye);
                        StartCoroutine("OnMgEffect");
                    }
                    break;
                case 15:
                    if (GetDie.MyEye <= 4)
                    {
                        The_Strongest_Hardness(GetDie.MyEye);
                    }
                    break;
                case 16:
                    if (GetDie.MyEye == 6)
                    {
                        Archmage(GetDie.MyEye);
                        StartCoroutine("OnMgEffect");
                    }
                    break;
                case 17:
                    if (GetDie.MyEye == 6)
                    {
                        Gladiator(GetDie.MyEye);
                        StartCoroutine("OnMgEffect");
                    }
                    break;
                case 18:
                    if (GetDie.MyEye == 6)
                    {
                        Aegis(GetDie.MyEye);
                    }
                    break;
                case 19:
                    if (GetDie.MyEye % 2 == 0)
                    {
                        Fight_Like_Hell(GetDie.MyEye);
                    }
                    break;
                case 20:
                    Predation(GetDie.MyEye);
                    break;
                case 21:
                    if (GetDie.MyEye <= 4)
                    {
                        Berserker(GetDie.MyEye);
                    }
                    break;
                case 22:
                    if (GetDie.MyEye == 6)
                    {
                        Athena(GetDie.MyEye);
                    }
                    break;
                case 23:
                    Like_A_Shadow(GetDie.MyEye);
                    break;
                case 24:
                    Blade_Of_Shadow(GetDie.MyEye);
                    break;
                case 25:
                    Fall_In_Darkness(GetDie.MyEye);
                    break;
                case 26:
                    Erebos(GetDie.MyEye);
                    break;
                case 27:
                    if (GetDie.MyEye % 2 == 1)
                        Poseidon(GetDie.MyEye);
                    break;
                case 28:
                    if (GetDie.MyEye % 2 == 0)
                        Zeus(GetDie.MyEye);
                    break;
                case 29:
                    if (GetDie.MyEye == 6)
                        Apollon(GetDie.MyEye);
                    break;
                case 30:
                    if (GetDie.MyEye == 6)
                    {
                        Gaia(GetDie.MyEye);
                    }
                    break;
                case 31:
                    if (GetDie.MyEye % 2 == 0)
                        Incapacitation(GetDie.MyEye);
                    break;
                case 32:
                    if (GetDie.MyEye % 2 == 0)
                        Archmage_Plus(GetDie.MyEye);
                    break;
                case 33:
                    Fireball_Plus(GetDie.MyEye);
                    break;
                case 34:
                    Invocation(GetDie.MyEye);
                    break;
                case 35:
                    Hermes(GetDie.MyEye);
                    break;
            }
            GameManager.GetInstance().PlusA(Dmg, DmgPlus, MgDmg, Armor, Heal, Miss, Ice, Fire, Eternal, DelHp, DelMyHp);
        }
        else
        {

            END = true;
        }
    }
    public void CardDel()
    {
        Destroy(this);
    }
    void Strike(int dice)//물리데미지 
    {

        Dmg += 2;

    }
    void Defend(int dice)//방어구획득
    {

        Armor += 2;
    }
    void Bash(int dice)//최대3 주사위 눈만큼공격
    {

        Dmg += dice;
    }
    void Simmering_Anger(int dice)// 이번턴 데미지 3증가
    {

        DmgPlus += 3;
    }
    void Flame(int dice)// 마법데미지 
    {

        MgDmg += 2;
    }
    //2티어
    void Increased_Defend(int dice)//주사위 눈만큼 방어
    {

        Armor += dice;
    }
    void Home_Thrust(int dice)//주사위 눈만큼 물리데미지
    {

        Dmg += dice;
    }
    void FireBall(int dice) //마법데미지 4
    {

        MgDmg += 4;
    }
    void Arrows(int dice)
    {

        Dmg += 4;
    }
    void Hammer(int dice)
    {

        GameManager.GetInstance().BreakArmor();
    }
    void HpHeal(int dice)
    {

        Heal += dice;
    }
    //3티어
    void Blade_Storm(int dice)//주사위 눈의 세 배 만큼 물리대미지 최대 4
    {
        Dmg += dice * 3;
    }
    void Body_Slam(int dice)//방어도 만큼 대미지 최소4
    {
        Dmg += Armor;
    }
    void Blizzard(int dice)//10 마법데미지 최소 4
    {
        MgDmg += 10;
    }
    void The_Strongest_Hardness(int dice)// 최대 4 주사위 눈의 세 배만큼 방어도 획득
    {
        Armor += dice * 3;
    }
    //4티어 무조건 6
    void Archmage(int dice)//10 마법 대미지 /n 10 방어도 획득 
    {
        MgDmg += 10;
        Armor += 10;
    }
    void Gladiator(int dice)//10 마법 대미지 /n 10 방어도 획득
    {
        Dmg += 10;
        Armor += 10;
    }
    void Aegis(int dice)//10 방어도 획득 /n 방어도 만큼 대미지
    {
        Armor += 10;
        Dmg += Armor;
    }
    //Warrior
    void Fight_Like_Hell(int dice)//10 물리 대미지 /n - 5 HP 짝수 
    {
        Dmg += 10;
        DelHp -= 5;
    }
    void Predation(int dice)//주사위 눈만큼 물리 대미지 /n 주사위 눈만큼 회복
    {
        Dmg += dice;
        Heal += dice;
    }
    void Berserker(int dice)// - 10 HP /n 게임 종료까지 공격 대미지 5증가 최소 4
    {
        DelMyHp -= 10;
    }
    void Athena(int dice) // 3개의 추가 다이스 /n 영원한 전투 이건 태웅님이 생각중임 
    {

    }
    void Like_A_Shadow(int dice)//회피 1회 /n 다이스 하나 받기
    {
        Miss = true;
    }
    void Blade_Of_Shadow(int dice)//2 마법 대미지를 입히고 다이스 하나 받기 /n 주사위 수가 짝수일 경우 카드를 재사용한다
    {
        MgDmg += 2;
    }
    void Fall_In_Darkness(int dice)//필드에 있는 카드 하나를 선택후 복제합니다 /n 다이스 하나 받기
    {

    }
    void Erebos(int dice)//3개의 추가 다이스 /n 영원한 어둠
    {

    }
    void Poseidon(int dice)//10 마법 대미지 /n 빙결 2
    {
        MgDmg += 10;
        Ice += 2;
    }
    void Zeus(int dice) //10 마법 대미지 /n 발화 2
    {
        MgDmg += 10;
        Fire += 2;
    }
    void Apollon(int dice) //주사위 수의 세 배만큼 HP 회복
    {
        Heal += dice * 3;
    }
    void Gaia(int dice)//3개의 추가 다이스 /n 영원한 접신
    {

    }
    void Incapacitation(int dice) // 방어구 파괴 짝수
    {
        GameManager.GetInstance().BreakArmor();

    }
    void Archmage_Plus(int dice) // 홀수 10마뎀 10 방어구
    {
        MgDmg += 10;
        Armor += 10;
    }
    void Fireball_Plus(int dice) // 10마뎀
    {
        MgDmg += 10;
    }
    void Invocation(int dice) // 카드세장중뽑기
    {

    }
    void Hermes(int dice) // 3개의 추가 다이스 /n 영원한 영창 무조건 6
    {

    }
    public IEnumerator OnMgEffect()
    {

        CARDMagEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        END = true;
    }
    public IEnumerator OnEffect()
    {

       
        for (int i = 0; i < 20; i++)
        {
            transform.position = Vector3.Lerp(Fpos.position, EnemyPlayer.transform.position, i * 0.05f);
            yield return new WaitForSeconds(0.0025f);
        }
        for (int i = 0; i < 20; i++)
        {
            transform.position = Vector3.Lerp(EnemyPlayer.transform.position, Fpos.position, i * 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);

        
        UIManager.GetInstance().SendATK();

        END = true;
        yield return new WaitForSeconds(0.1f);
        END = true;

    }
    private void OnMouseDown()
    {
        CARD.SetActive(true);
    }
    private void OnMouseUp()
    {
        CARD.SetActive(false);
    }
}
