using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
public partial class MatchManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void JoinMatchServer()
    {
        if (isConnectMatchServer)
        {
            return;
        }
        ErrorInfo errorinfo;
        isConnectMatchServer = true;
        if (!Backend.Match.JoinMatchMakingServer(out errorinfo))
        {
            Debug.Log("매칭 실패");
            Debug.Log("매칭 실패");
        }
    }
 
    public void LeaveMatchServer()
    {
        StartTimer = false;
        isConnectMatchServer = false;
        Backend.Match.LeaveMatchMakingServer();
    }

    public bool CreateMatchRoom_()
    {
        if (!isConnectMatchServer)
        {
            JoinMatchServer();
            return false;
        }
        Backend.Match.CreateMatchRoom();
        return true;
    }
    public void LeaveMatch_Room()
    {
        Backend.Match.LeaveMatchRoom();
    }
    public void RequestMatchMaking(int index)
    {
        if (!isConnectMatchServer)
        {
            JoinMatchServer();
            return;
        }
        isConnectInGameServer = false;
        Backend.Match.RequestMatchMaking(matchInfos[0].matchType, matchInfos[0].matchModeType, matchInfos[0].inDate);
        if (isConnectInGameServer)
        {

            Backend.Match.LeaveGameServer();
        }
    }
    public void CancelRegistMatchMaking()
    {
        Backend.Match.CancelMatchMaking();
    }
    private void ProcessMatchMakinRespone(MatchMakingResponseEventArgs args)
    {
        bool isError = false;

        switch (args.ErrInfo)
        {
            case ErrorCode.Success:
                // 매칭 성공했을 때
                // debugLog = string.Format(SUCCESS_MATCHMAKE, args.Reason);
                //  LobbyUI.GetInstance().MatchDoneCallback();
                ProcessMatchSuccess(args);
                break;
            case ErrorCode.Match_InProgress:
                // 매칭 신청 성공했을 때 or 매칭 중일 때 매칭 신청을 시도했을 때
                Debug.Log("gdgd");

                // 매칭 신청 성공했을 때

                if (args.Reason == string.Empty)
                {
                    Debug.Log("ㅎㅇㅎㅇ");
                    StartTimer = true;
                    //    debugLog = SUCCESS_REGIST_MATCHMAKE;
                    //   LobbyUI.GetInstance().MatchRequestCallback(true);
                }
                break;
            case ErrorCode.Match_MatchMakingCanceled:
                // 매칭 신청이 취소되었을 때
               // debugLog = string.Format(CANCEL_MATCHMAKE, args.Reason);

               // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidMatchType:
                isError = true;
                // 매치 타입을 잘못 전송했을 때
               // debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVAILD_MATCHTYPE);

                //LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidModeType:
                isError = true;
                // 매치 모드를 잘못 전송했을 때
                //debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVALID_MODETYPE);

               // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.InvalidOperation:
                isError = true;
                // 잘못된 요청을 전송했을 때
                //debugLog = string.Format(INVALID_OPERATION, args.Reason);
               // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_Making_InvalidRoom:
                isError = true;
                // 잘못된 요청을 전송했을 때
                //debugLog = string.Format(INVALID_OPERATION, args.Reason);
                //LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Exception:
                isError = true;
                // 매칭 되고, 서버에서 방 생성할 때 에러 발생 시 exception이 리턴됨
                // 이 경우 다시 매칭 신청해야 됨
                //debugLog = string.Format(EXCEPTION_OCCUR, args.Reason);

               // LobbyUI.GetInstance().RequestMatch();
                break;
        }

       // if (!debugLog.Equals(string.Empty))
        //{
        //    Debug.Log(debugLog);
        //    if (isError == true)
      //      {
         //       LobbyUI.GetInstance().SetErrorObject(debugLog);
       //     }
        //}
    }
}
