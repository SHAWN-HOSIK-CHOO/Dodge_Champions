using Epic.OnlineServices;
using Epic.OnlineServices.Presence;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
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
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _guestLogin.onClick.AddListener(OnGuestLogin);
        _epicPortalLogin.onClick.AddListener(OnEpicPortalLogin);
        _developerLogin.onClick.AddListener(OnDeveloperLogin);
    }

    void OnLoginSuccess(ProductUserId localPUID)
    {
        var _eosNet = SingletonMonoBehaviour<EOS_Core>._instance;
        FreeNet._instance._localUser.SetlocaPUID(localPUID.ToString());
        SceneManager.LoadScene("LobbyScene");
    }

    void OnGuestLogin()
    {
        var _eosNet = SingletonMonoBehaviour<EOS_Core>._instance;
        string username = "I_AM_User";
        EOSWrapper.ConnectControl.DeviceIDConnect(_eosNet._IConnect, username, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info)=>
        {
            if(info.ResultCode == Epic.OnlineServices.Result.AccessDenied)
            {
                // 에픽 브랜드 리뷰를 받지 못해 아직 지원 안됨 
            }
            if(info.ResultCode == Epic.OnlineServices.Result.NotFound)
            {
                EOSWrapper.ConnectControl.CreateDeviceID(_eosNet._IConnect,(ref Epic.OnlineServices.Connect.CreateDeviceIdCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.Success)
                    {
                        EOSWrapper.ConnectControl.DeviceIDConnect(_eosNet._IConnect, username);
                    }
                });
            }
            else if (info.ResultCode == Epic.OnlineServices.Result.Success)
            {
                OnLoginSuccess(info.LocalUserId);
            }
        });
    }
    void OnEpicPortalLogin()
    {
        var _eosNet = SingletonMonoBehaviour<EOS_Core>._instance;
        EOSWrapper.LoginControl.EpicPortalLogin(_eosNet._IAuth, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if (info.ResultCode == Epic.OnlineServices.Result.Success)
            {
                EOSWrapper.ConnectControl.EpicIDConnect(_eosNet._IAuth, _eosNet._IConnect, info.LocalUserId, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.Success)
                    {
                        OnLoginSuccess(info.LocalUserId);
                    }
                    else if (info.ResultCode == Epic.OnlineServices.Result.InvalidUser)
                    {
                        EOSWrapper.ConnectControl.CreateUser(_eosNet._IConnect, info.ContinuanceToken, (ref Epic.OnlineServices.Connect.CreateUserCallbackInfo info) =>
                        {
                            if (info.ResultCode == Epic.OnlineServices.Result.Success)
                            {
                                OnLoginSuccess(info.LocalUserId);
                            }
                        });
                    }
                });
            }
        });

    }
    void OnDeveloperLogin()
    {
        var _eosNet = SingletonMonoBehaviour<EOS_Core>._instance;
        EOSWrapper.LoginControl.DeveloperToolLogin(_eosNet._IAuth, _host, _credential, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if (info.ResultCode == Epic.OnlineServices.Result.Success)
            {
                EOSWrapper.ConnectControl.EpicIDConnect(_eosNet._IAuth, _eosNet._IConnect, info.LocalUserId, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.Success)
                    {
                        OnLoginSuccess(info.LocalUserId);
                    }
                    else if (info.ResultCode == Epic.OnlineServices.Result.InvalidUser)
                    {
                        EOSWrapper.ConnectControl.CreateUser(_eosNet._IConnect, info.ContinuanceToken, (ref Epic.OnlineServices.Connect.CreateUserCallbackInfo info) =>
                        {
                            if (info.ResultCode == Epic.OnlineServices.Result.Success)
                            {
                                OnLoginSuccess(info.LocalUserId);
                            }
                        });
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
