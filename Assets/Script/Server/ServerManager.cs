using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using static BackEnd.SendQueue;
using UnityEngine.SocialPlatforms;
using System;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ServerManager : MonoBehaviour
{
    public GameObject obj;
    BackEnd.Tcp.MatchMakingInteractionEventArgs args;
    private static ServerManager instance;
    public bool isLogin { get; private set; }
    private string tempNickName;
    public string myNickName { get; private set; } = string.Empty;
    public string myIndate { get; private set; } = string.Empty;
    private Action<bool, string> LogInSuccessFunc = null;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public static ServerManager GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("ServerManager 인스턴스없음");
            return null;
        }
        return instance;
    }
    void Start()
    {
        isLogin = false;
        try
        {
            Backend.Initialize(() =>
            {
                if (Backend.IsInitialized)
                {
                    Debug.Log("뒤긑 초기화 성공");
                    StartSendQueue(true);
                    Debug.Log(Backend.Utils.GetGoogleHash());

                }
                else
                {
                    Debug.Log("뒤끝 초기화 실패");
                }
            });
        }
        catch (Exception e)
        {
            Debug.Log("[예외] 뒤끝 초기화 실패\n" + e.ToString());
        }
    }
    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        StopSendQueue();
    }

    void OnApplicationPause(bool isPause)
    {
        Debug.Log("OnApplicationPause : " + isPause);
        if (isPause == false)
        {
            ResumeSendQueue();
        }
        else
        {
            PauseSendQueue();
        }
    }
    void Update()
    {
        SendQueue.Poll();
    }
    public void UpdateNickname(string nickname, Action<bool, string> func)
    {
        Enqueue(Backend.BMember.UpdateNickname, nickname, bro =>
        {
            if (!bro.IsSuccess())
            {
                Debug.LogError("닉네임 생성 실패\n" + bro.ToString());
                func(false, string.Format("statusCode : {0}\nErrorCode : {1}\nMessage : {2}",
                    bro.GetStatusCode(), bro.GetErrorCode(), bro.GetMessage()));
                return;
            }
            LogInSuccessFunc = func;
            OnBackendAuthorized();
        });
    }

    public void GuestLogin(Action<bool, string> func)
    {

        Enqueue(Backend.BMember.GuestLogin, callback =>
         {
             if (callback.IsSuccess())
             {
                 Debug.Log("게스트 로그인 성공");
                 LogInSuccessFunc = func;
                 OnPrevBackendAuthorized();
                 return;
             }
             Debug.Log("게스트 로그인 실패 \n" + callback);
             func(false, string.Format("statusCode : {0}\nErrorCode : {1}\nMessage : {2}", callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
         });
    }
    private void OnPrevBackendAuthorized()
    {
        isLogin = true;
        OnBackendAuthorized();
    }
    private void OnBackendAuthorized()
    {
        Enqueue(Backend.BMember.GetUserInfo, callback =>
         {
             if(!callback.IsSuccess())
             {
                 Debug.LogError("유저 정보 불러오기실패\n" + callback);
                 LogInSuccessFunc(false, string.Format("statusCode : {0}\nErrorCode : {1}\nMessage : {2}", callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
                 return;
             }
             Debug.Log("유저정보 \n"+callback);

             var info = callback.GetReturnValuetoJSON()["row"];
             if(info["nickname"]==null)
             {
                 LoginUI.GetInstance().NickName.SetActive(true);


             }
             myNickName = info["nickname"].ToString();
             myIndate = info["inDate"].ToString();

             SceneManager.LoadScene("TitleScene");
             if(LogInSuccessFunc != null)
             {
                 MatchManager.GetInstance().GetMatchList(LogInSuccessFunc);
                 LogInSuccessFunc(true, string.Empty);
             }
         }
        );
    }
    public void CustomLogin(string id, string pw, Action<bool, string> func)
    {
        Enqueue(Backend.BMember.CustomLogin, id, pw, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("커스텀 로그인 성공");
                LogInSuccessFunc = func;

                OnPrevBackendAuthorized();
                return;
            }

            Debug.Log("커스텀 로그인 실패\n" + callback);
            func(false, string.Format("statusCode : {0}\nErrorCode : {1}\nMessage : {2}",
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
        });
    }

    // 커스텀 회원가입
    public void CustomSignIn(string id, string pw, Action<bool, string> func)
    {
        tempNickName = id;
        Enqueue(Backend.BMember.CustomSignUp, id, pw, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("커스텀 회원가입 성공");
                LogInSuccessFunc = func;

                OnPrevBackendAuthorized();
                return;
            }

            Debug.LogError("커스텀 회원가입 실패\n" + callback.ToString());
            func(false, string.Format("statusCode : {0}\nErrorCode : {1}\nMessage : {2}",
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
        });
    }
    // Update is called once per frame

}
