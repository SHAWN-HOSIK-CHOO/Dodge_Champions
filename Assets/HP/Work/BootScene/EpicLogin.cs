using Epic.OnlineServices;
using Epic.OnlineServices.UserInfo;
using System;
using UnityEngine;

public class EpicLogin : MonoBehaviour
{
    public event Action<Result> onLogin;
    public event Action<Result> onConnect;
    protected void OnLoginComplete(Result result, EpicAccountId localEAID)
    {
        if (EOSWrapper.ETC.ErrControl(result, onLogin))
        {
            FreeNet.Instance._localUser._localEAID = new EOSWrapper.ETC.EAID(localEAID);
            EOSWrapper.UserInfo.QueryAndCopyUserInfo(FreeNet.Instance._eosCore._IUSER, localEAID, localEAID, (Result result, UserInfoData? data) =>
            {
                if (result == Result.Success)
                {
                    FreeNet.Instance._localUser._localUserInfo = data;
                }
                onLogin?.Invoke(result);
            });
        }
    }
    protected void OnConnectComplete(Result result, ProductUserId localPUID)
    {
        if (EOSWrapper.ETC.ErrControl(result, onConnect))
        {
            FreeNet.Instance._localUser._localPUID = new EOSWrapper.ETC.PUID(localPUID);
            onConnect?.Invoke(result);
        }
    }
}
