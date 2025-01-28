using Epic.OnlineServices;
using Epic.OnlineServices.Presence;
using System.Collections;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
    WaitingTransitionUI _waitingTransitionUI;
    FreeNet _freeNet;

    [SerializeField]
    Button _guestLogin;

    [SerializeField]
    Button _epicPortalLogin;

    #region For Developer
    [SerializeField]
    Button _developerLogin;
    [SerializeField]
    string _host;
    [SerializeField]
    string _credential;
    #endregion

    private IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<WaitingTransitionUI>.WaitInitialize();
        _waitingTransitionUI = WaitingTransitionUI._instance;
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _freeNet = FreeNet._instance;

        _guestLogin.onClick.AddListener(OnGuestLogin);
        _epicPortalLogin.onClick.AddListener(OnEpicPortalLogin);
        _developerLogin.onClick.AddListener(OnDeveloperLogin);
    }

    void OnLoginSuccess(Result result, ProductUserId localPUID)
    {
        if (result == Result.Success)
        {
            _freeNet._localUser.SetlocaPUID(localPUID.ToString());
            _waitingTransitionUI.UpdateWaitInfoDetail($"LoginSuccess");

            _waitingTransitionUI._transitionUI.MakeTransitionEnd("Login");
            _waitingTransitionUI._transitionUI.AddNullTransition("Load Lobby", _waitingTransitionUI.UpdateWaitInfo);
            _waitingTransitionUI.UpdateWaitInfoDetail($"Load Lobby...");
            SceneManager.LoadScene("LobbyScene");
        }
        else
        {
            _waitingTransitionUI.UpdateWaitInfoDetail($"Fail... {result}");
            _waitingTransitionUI._transitionUI.MakeTransitionEnd("Login");
        }
    }

    void OnGuestLogin()
    {
        _waitingTransitionUI._transitionUI.AddNullTransition("Login",_waitingTransitionUI.UpdateWaitInfo);
        string username = "I_AM_User";
        EOSWrapper.ConnectControl.DeviceIDConnect(_freeNet._eosCore._IConnect, username, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info)=>
        {
            if(info.ResultCode == Epic.OnlineServices.Result.NotFound)
            {
                EOSWrapper.ConnectControl.CreateDeviceID(_freeNet._eosCore._IConnect,(ref Epic.OnlineServices.Connect.CreateDeviceIdCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.Success)
                    {
                        EOSWrapper.ConnectControl.DeviceIDConnect(_freeNet._eosCore._IConnect, username);
                    }
                });
            }
            else if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnLoginSuccess))
            {
                OnLoginSuccess(Result.Success, info.LocalUserId);
            }
        });
    }
    void OnEpicPortalLogin()
    {
        _waitingTransitionUI._transitionUI.AddNullTransition("Login",_waitingTransitionUI.UpdateWaitInfo);
        EOSWrapper.LoginControl.EpicPortalLogin(_freeNet._eosCore._IAuth, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if(EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnLoginSuccess))
            {
                EOSWrapper.ConnectControl.EpicIDConnect(_freeNet._eosCore._IAuth, _freeNet._eosCore._IConnect, info.LocalUserId, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.InvalidUser)
                    {
                        EOSWrapper.ConnectControl.CreateUser(_freeNet._eosCore._IConnect, info.ContinuanceToken, (ref Epic.OnlineServices.Connect.CreateUserCallbackInfo info) =>
                        {
                            if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnLoginSuccess))
                            {
                                OnLoginSuccess(Result.Success, info.LocalUserId);
                            }
                        });
                    }
                    else if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnLoginSuccess))
                    {
                        OnLoginSuccess(Result.Success, info.LocalUserId);
                    }
                });
            }
        });

    }
    void OnDeveloperLogin()
    {
        _waitingTransitionUI._transitionUI.AddNullTransition("Login", _waitingTransitionUI.UpdateWaitInfo);
        EOSWrapper.LoginControl.DeveloperToolLogin(_freeNet._eosCore._IAuth, _host, _credential, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnLoginSuccess))
            {
                EOSWrapper.ConnectControl.EpicIDConnect(_freeNet._eosCore._IAuth, _freeNet._eosCore._IConnect, info.LocalUserId, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.InvalidUser)
                    {
                        EOSWrapper.ConnectControl.CreateUser(_freeNet._eosCore._IConnect, info.ContinuanceToken, (ref Epic.OnlineServices.Connect.CreateUserCallbackInfo info) =>
                        {
                            if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnLoginSuccess))
                            {
                                OnLoginSuccess(Result.Success,info.LocalUserId);
                            }
                        });
                    }
                    else if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnLoginSuccess))
                    {
                        OnLoginSuccess(Result.Success, info.LocalUserId);
                    }

                });
            }
        });
    }

    private void OnDestroy()
    {
        _guestLogin.onClick.RemoveAllListeners();
        _epicPortalLogin.onClick.RemoveAllListeners();
        _developerLogin.onClick.RemoveAllListeners();
    }
}
