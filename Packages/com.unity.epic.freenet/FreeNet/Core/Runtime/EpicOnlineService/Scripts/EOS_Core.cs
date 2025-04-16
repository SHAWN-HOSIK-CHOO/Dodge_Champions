using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.P2P;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.PlayerDataStorage;
using Epic.OnlineServices.RTC;
using Epic.OnlineServices.RTCAudio;
using Epic.OnlineServices.Sessions;
using Epic.OnlineServices.TitleStorage;
using Epic.OnlineServices.UI;
using Epic.OnlineServices.UserInfo;
using System.Collections;
using UnityEditor;
using UnityEngine;
public partial class EOS_Core : MonoBehaviour
{
    public enum SDKState
    {
        Released,
        Initialized,
    }
    public SDKState _sdkState { get; private set; }
    #region EOS Interface
    public PlatformInterface _IPlatform { get; private set; }
    public AuthInterface _IAuth { get; private set; }
    public ConnectInterface _IConnect { get; private set; }
    public LobbyInterface _ILobby { get; private set; }
    public SessionsInterface _ISession { get; private set; }
    public RTCInterface _IRTC { get; private set; }
    public RTCAudioInterface _IRTCAUDIO { get; private set; }
    public P2PInterface _IP2P { get; private set; }
    public UIInterface _IUI { get; private set; }
    public UserInfoInterface _IUSER { get; private set; }

    public PlayerDataStorageInterface _IDATA { get; private set; }
    public TitleStorageInterface _ITitle { get; private set; }

    public EOS_Credential _credential { get; private set; }

    #endregion
    #region Credentials
    [SerializeField]
    private EOS_Credential _dev;
    [SerializeField]
    private EOS_Credential _stage;
    [SerializeField]
    private EOS_Credential _live;
    [SerializeField]
    public EOS_Credential.CredentialType type;
    #endregion

    [SerializeField]
    LogCategory category;
    [SerializeField]
    LogLevel level;
    Coroutine _tick;
    private IEOS_PlatformFactory _factory;
    public void Run()
    {
        if (_sdkState == SDKState.Released)
        {
            _sdkState = Init();
            if (_sdkState == SDKState.Initialized)
            {
                _tick = StartCoroutine(Tick());
            }
        }
    }
    void Stop()
    {
        StopCoroutine(_tick);
        Dispose();
    }

    void Dispose()
    {
        if (_sdkState == SDKState.Initialized)
        {
            RleasePartialClass();
            _factory.Dispose();
            _sdkState = SDKState.Released;
        }
    }
#if UNITY_EDITOR
    public void OnBeforeAssemblyReload()
    {
        AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        Dispose();
    }
#endif

    SDKState Init()
    {
#if UNITY_EDITOR
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif
        _factory = EOS_Factory.GetFactory();
        if (!_factory.LoadDLL()) return SDKState.Released;
        _credential = null;
        if (type == EOS_Credential.CredentialType.Dev)
        {
            _credential = _dev;
        }
        else if (type == EOS_Credential.CredentialType.Stage)
        {
            _credential = _stage;
        }
        else if (type == EOS_Credential.CredentialType.Live)
        {
            _credential = _live;
        }
        if (!_factory.MakePlatform(_dev, category, level, out var IPlatform))
        {
            return SDKState.Released;
        }
        _IPlatform = IPlatform;
        _IAuth = _IPlatform.GetAuthInterface();
        _IConnect = _IPlatform.GetConnectInterface();
        _IP2P = _IPlatform.GetP2PInterface();
        _ILobby = _IPlatform.GetLobbyInterface();
        _ISession = _IPlatform.GetSessionsInterface();
        _IRTC = _IPlatform.GetRTCInterface();
        _IRTCAUDIO = _IRTC.GetAudioInterface();
        _IUI = _IPlatform.GetUIInterface();
        _IUSER = _IPlatform.GetUserInfoInterface();
        _IDATA = _IPlatform.GetPlayerDataStorageInterface();
        _ITitle = _IPlatform.GetTitleStorageInterface();
        InitPartialClass();
        EOSWrapper.ETC.SetApplicationStatus(_IPlatform, ApplicationStatus.Foreground);
        return SDKState.Initialized;
    }

    void InitPartialClass()
    {
        InitP2P();
        InitLogin();
        InitConnect();
    }
    void RleasePartialClass()
    {
        try
        {
            ReleaseP2P();
            ReleaseConnect();
            ReleaseLogin();
        }
        catch
        {
            Debug.LogError("RleasePartialClass Failed");
        }
    }

    IEnumerator Tick()
    {
        while (true)
        {
            _IPlatform?.Tick();
            ReceivePacket();
            yield return null;
        }
    }

    private void OnApplicationQuit()
    {
        Stop();
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            EOSWrapper.ETC.SetApplicationStatus(_IPlatform, ApplicationStatus.BackgroundSuspended);
        }
        else
        {
            EOSWrapper.ETC.SetApplicationStatus(_IPlatform, ApplicationStatus.Foreground);
        }
    }
}
