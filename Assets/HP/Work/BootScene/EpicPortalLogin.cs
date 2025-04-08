using DG.Tweening;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.UserInfo;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EpicPortalLogin : EpicLogin
{
    [SerializeField]
    UIImgButton _LoginButton;


    void Start()
    {
        _LoginButton.OnPointerClickAction += OnLoginBGClick;
    }

    private void OnLoginBGClick(BaseEventData arg)
    {
        _LoginButton.DeActivate();
        OnPersistentLogin();
        DOVirtual.DelayedCall(3f, () => _LoginButton.Activate());
    }
    void OnEpicPortalLogin()
    {
        var options = new DeletePersistentAuthOptions();
        FreeNet._instance._eosCore._IAuth.DeletePersistentAuth(ref options, null, null);
        EOSWrapper.LoginControl.EpicPortalLogin(FreeNet._instance._eosCore._IAuth, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if (EOSWrapper.ETC.ErrControl<EpicAccountId>(info.ResultCode, OnLoginComplete))
            {
                OnLoginComplete(Result.Success, info.LocalUserId);
                EOSWrapper.ConnectControl.EpicIDConnect(FreeNet._instance._eosCore._IAuth, FreeNet._instance._eosCore._IConnect, info.LocalUserId, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.InvalidUser)
                    {
                        EOSWrapper.ConnectControl.CreateUser(FreeNet._instance._eosCore._IConnect, info.ContinuanceToken, (ref Epic.OnlineServices.Connect.CreateUserCallbackInfo info) =>
                        {
                            if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnConnectComplete))
                            {
                                OnConnectComplete(Result.Success, info.LocalUserId);
                            }
                        });
                    }
                    else if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnConnectComplete))
                    {
                        OnConnectComplete(Result.Success, info.LocalUserId);
                    }
                });
            }
        });

    }
    void OnPersistentLogin()
    {
        EOSWrapper.LoginControl.PersistentLogin(FreeNet._instance._eosCore._IAuth, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if(info.ResultCode == Result.InvalidAuth)
            {
                OnEpicPortalLogin();
            }
            else if(info.ResultCode ==  Result.AuthInvalidRefreshToken)
            {
                OnEpicPortalLogin();
            }
            OnLoginComplete(info.ResultCode, info.LocalUserId);
        });
    }
}

