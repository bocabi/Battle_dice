using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using BackEnd.Tcp;
using BackEnd;

public class _Player : MonoBehaviour
{
    private SessionId index1 = 0;
    private string nickName1 = string.Empty;
    private bool isMe1 = false;
    public bool isLive1 = false;
    public int Hp { get;  set; } = 30;
    public int hpEnemy = 30;
    public int Shield { get; private set; } = 0;
    public void Initialize(bool isMe,SessionId index, string nickName)
    {
        isMe1 = isMe;
        index1 = index;
        nickName1 = nickName;
        this.isLive1 = true;
        Hp = 30;
        Shield = 0;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public bool GetIsLive()
    {
        return isLive1;
    }
    public SessionId GetIndex()
    {
        return index1;
    }
    public string GetNickName()
    {
        return nickName1;
    }

    public bool IsMe()
    {
        return isMe1;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
