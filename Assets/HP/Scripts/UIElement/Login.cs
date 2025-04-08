using DG.Tweening;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.UserInfo;
using HP;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class Login : MonoBehaviour
{
    [SerializeField]
    Image _background;
    [SerializeField]
    Button _guestLogin;
    [SerializeField]
    Button _epicPortalLogin;
    [SerializeField]
    Button _developerLogin;
    [SerializeField]
    SystemAnounce _systemAnouncePref;
    [SerializeField]
    ConsoleController _consoleController;

    string _LoginID = "localhost:8000";
    string _LoginCredential = "1";

    public event Action onLogin;
    public event Action onConnect;
    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _guestLogin.onClick.AddListener(OnGuestLogin);
        _epicPortalLogin.onClick.AddListener(OnEpicPortalLogin);
        _developerLogin.onClick.AddListener(OnDeveloperLogin);
        gameObject.SetActive(true);
        _consoleController.onSubmit += OnSubmit;

    }
    void OnSubmit(TMPInputField.IInputMode mode, string text)
    {
        var inputmode = (ConsoleController.Mode)mode;
        if (inputmode._mode == ConsoleController.Mode.InputMode.S)
        {
            string[] parts = text.Split(' ', 3);
            if (parts.Length == 3)
            {
                string command = parts[0];
                if (command == "Developer_Login")
                {
                    _LoginID = parts[1];
                    _LoginCredential = parts[2];
                    var simulator = _consoleController.GetComponent<ConsoleSimulator>();
                    simulator.BeginTracking();
                    _consoleController.AddText($"<color=#00FFFF>Command : Developer_Login</color>");
                    simulator.EndTracking();
                    simulator.Simulate(0.05f);
                    OnDeveloperLogin();
                }
            }
        }
    }
    void OnLoginComplete(Result result, EpicAccountId localEAID)
    {
        if (result == Result.Success)
        {
            FreeNet._instance._localUser._localEAID = new EOSWrapper.ETC.EAID(localEAID);

            var simulator = _consoleController.GetComponent<ConsoleSimulator>();
            simulator.BeginTracking();
            _consoleController.AddText($"QueryAndCopyUserInfo...");
            simulator.EndTracking();
            simulator.Simulate(0.05f);

            EOSWrapper.UserInfo.QueryAndCopyUserInfo(FreeNet._instance._eosCore._IUSER, localEAID, localEAID, (Result result, UserInfoData? data) =>
            {
                if (result == Result.Success)
                {
                    FreeNet._instance._localUser._localUserInfo = data;
                    onLogin?.Invoke();
                }
                else
                {
                    ShowErr(result.ToString());
                }
            });
        }
        else
        {
            ShowErr(result.ToString());
        }
    }
    void ShowErr(string msg)
    {
        var seq = DOTween.Sequence();
        seq.Append(_background.DOColor(Color.red, 0.2f).From(Color.white));
        seq.AppendInterval(0.2f);
        seq.Append(_background.DOColor(Color.white, 0.2f)).OnComplete(() =>
        {
            Instantiate(_systemAnouncePref).GetComponent<SystemAnounce>().Show(msg);
        });
    }
    void OnConnectComplete(Result result, ProductUserId localPUID)
    {
        if (result == Result.Success)
        {
            FreeNet._instance._localUser._localPUID = new EOSWrapper.ETC.PUID(localPUID);
            var seq = DOTween.Sequence();
            seq.Append(_background.DOColor(Color.green, 0.2f).From(Color.white));
            seq.AppendInterval(0.2f);
            seq.Append(_background.DOColor(Color.white, 0.2f));
            GetComponent<CanvasGroup>().DOFade(0, 0.2f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                onConnect?.Invoke();
            });
        }
        else
        {
            ShowErr(result.ToString());
        }
        Debug.Log($"Coonect RESULT : {result}");
    }
    void OnGuestLogin()
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText($"OnGuestLogin Start...");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        var username = "Unkown";
        EOSWrapper.ConnectControl.DeviceIDConnect(FreeNet._instance._eosCore._IConnect, username, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
        {
            if (info.ResultCode == Epic.OnlineServices.Result.NotFound)
            {
                EOSWrapper.ConnectControl.CreateDeviceID(FreeNet._instance._eosCore._IConnect, (ref Epic.OnlineServices.Connect.CreateDeviceIdCallbackInfo info) =>
                {
                    if (info.ResultCode == Epic.OnlineServices.Result.Success)
                    {
                        EOSWrapper.ConnectControl.DeviceIDConnect(FreeNet._instance._eosCore._IConnect, username);
                    }
                });
            }
            else if (EOSWrapper.ETC.ErrControl<ProductUserId>(info.ResultCode, OnConnectComplete))
            {
                OnConnectComplete(Result.Success, info.LocalUserId);
            }
        });
    }
    void OnEpicPortalLogin()
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText($"OnEpicPortalLogin Start...");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        EOSWrapper.LoginControl.EpicPortalLogin(FreeNet._instance._eosCore._IAuth, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
        {
            if (EOSWrapper.ETC.ErrControl<EpicAccountId>(info.ResultCode, OnLoginComplete))
            {
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
    void OnDeveloperLogin()
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText($"OnDeveloperLogin Start...");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        EOSWrapper.LoginControl.DeveloperToolLogin(FreeNet._instance._eosCore._IAuth, _LoginID, _LoginCredential, (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) =>
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
    private void OnDestroy()
    {
        _guestLogin.onClick.RemoveAllListeners();
        _epicPortalLogin.onClick.RemoveAllListeners();
        _developerLogin.onClick.RemoveAllListeners();
    }
}
