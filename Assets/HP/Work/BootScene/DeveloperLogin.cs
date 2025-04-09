using DG.Tweening;
using Epic.OnlineServices;
using Epic.OnlineServices.UserInfo;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DeveloperLogin : EpicLogin
{
    [SerializeField]
    UIImgButton _LoginButton;
    [SerializeField]
    string _id;
    [SerializeField]
    string _credential;


    void Start()
    {
        _LoginButton.OnPointerClickAction += OnLoginBGClick;
    }
    private void OnLoginBGClick(BaseEventData arg)
    {
        _LoginButton.DeActivate();
        OnDeveloperLogin(_id, _credential);
        DOVirtual.DelayedCall(3f, () => _LoginButton.Activate());
    }
    void OnDeveloperLogin(string id , string credential)
    {
        EOSWrapper.LoginControl.DeveloperToolLogin(FreeNet.Instance._eosCore._IAuth, id, credential, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if (EOSWrapper.ETC.ErrControl<EpicAccountId>(info.ResultCode, OnLoginComplete))
            {
                OnLoginComplete(Result.Success, info.LocalUserId);
                EOSWrapper.ConnectControl.EpicIDConnect(FreeNet.Instance._eosCore._IAuth, FreeNet.Instance._eosCore._IConnect, info.LocalUserId, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.InvalidUser)
                    {
                        EOSWrapper.ConnectControl.CreateUser(FreeNet.Instance._eosCore._IConnect, info.ContinuanceToken, (ref Epic.OnlineServices.Connect.CreateUserCallbackInfo info) =>
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
}
