using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainUI : MonoBehaviour
{
    public GameObject obj;
    public GameObject timertext;
    private int timer;
    private string timerStr;
    // Start is called before the first frame update
    void Start()
    {
        obj.GetComponent<Text>().text = ServerManager.GetInstance().myNickName;
    }

    // Update is called once per frame
    void Update()
    {
        if(MatchManager.GetInstance().Min.ToString().Length  == 1)
        {
        timerStr = MatchManager.GetInstance().Min.ToString("D"+2) + " : " ;
        }
        else
        {
            timerStr = MatchManager.GetInstance().Min.ToString() + " : ";

        }
        timer = (int)MatchManager.GetInstance().timer;
        if (timer.ToString().Length == 1)
        {

        timerStr += timer.ToString("D" + 2);
            timertext.GetComponent<Text>().text = timerStr;
        }
        else
        {
            timerStr += timer.ToString();
              timertext.GetComponent<Text>().text = timerStr;
        }
    }
    public void CancelMatching()
    {
        MatchManager.GetInstance().LeaveMatchServer();
    }
    public void StartMatching()
    {
        MatchManager.GetInstance().JoinMatchServer();
    }
}
