#if UNITY_EDITOR
#define EOS_DYNAMIC_BINDINGS
#endif
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.P2P;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.RTC;
using Epic.OnlineServices.RTCAudio;
using Epic.OnlineServices.Sessions;
using Epic.OnlineServices.UI;
using Epic.OnlineServices.UserInfo;
using UnityEngine;
public partial class EOSNet : SingletonMonoBehaviour<EOSNet>
{
    public enum InitState
    {
        None,
        Fail,
        Suceess,
    }
    public InitState _InitState { get; private set; }
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
    #endregion
    #region Credentials
    [SerializeField]
    private Credential _dev;
    [SerializeField]
    private Credential _stage;
    [SerializeField]
    private Credential _live;
    [SerializeField]
    public Credential.CredentialType type;
    #endregion
    private IEOSPlatformFactory _factory;
    private void Awake()
    {
        if ((this as Singleton<EOSNet>).Singleton(this))
        {
            Init();
        }
        else
        {
            Destroy(this);
        }
    }
    InitState Init()
    {
        _factory = EOSFactory.GetFactory();
        if(!_factory.LoadDLL()) return InitState.Fail;

        Credential credential = null;
        if (type == Credential.CredentialType.Dev)
        {
            credential = _dev;
        }
        else if (type == Credential.CredentialType.Stage)
        {
            credential = _stage;
        }
        else if (type == Credential.CredentialType.Live)
        {
            credential = _live;
        }
        if(!_factory.MakePlatform(_dev, out var IPlatform))
        {
            _factory.UnLoadDLL();
            return InitState.Fail;
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
        EOSWrapper.SetApplicationStatus(_IPlatform, ApplicationStatus.Foreground);

        _InitState = InitState.Suceess;
        return InitState.Suceess;
    }
    void Update()
    {
        _IPlatform?.Tick();
        ReceivePacket();
    }
    void Release()
    {
        //올바르게 정리되지 않을 시 에디터 프리징 현상이 발생
        _IPlatform?.Release();
        PlatformInterface.Shutdown();
        _factory?.UnLoadDLL();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus)
        {
            EOSWrapper.SetApplicationStatus(_IPlatform, ApplicationStatus.Foreground);
        }
        else
        {
            EOSWrapper.SetApplicationStatus(_IPlatform,ApplicationStatus.BackgroundSuspended);
        }
    }
    void OnApplicationPause(bool pauseStatus)
    {
        EOSWrapper.SetApplicationStatus(_IPlatform, ApplicationStatus.BackgroundSuspended);
    }
    private void OnDestroy()
    {
        Release();
    }
    private void OnApplicationQuit()
    {
        Destroy(this);
    }
}
