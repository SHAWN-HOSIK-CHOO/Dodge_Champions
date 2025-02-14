using Epic.OnlineServices;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyManager : SingletonMonoBehaviour<LobbyManager>
{
    [SerializeField]
    GameObject _lobbyInterfaceUIPref;
    GameObject _lobbyInterfaceUI;


    LobbyControl _lobbyControl;
    TextMeshProUGUI _joinStateUI;

    FreeNet _freeNet;
    TransitionUI _transitionUI;
    BasicUI _basicUI;
    EOS_SingleLobbyManager.EOS_Lobby _lastLobby;
    OpenLobbyUIKeyBinding _openLobbyUIKeyBinding;
    //InputBinding _inputBinding;
    private void Awake()
    {
        SingletonSpawn(this);
    }
    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        yield return SingletonMonoBehaviour<SingletonCanvas>.WaitInitialize();
        _freeNet = FreeNet._instance;
        _transitionUI = SingletonCanvas._instance.GetComponentInChildren<TransitionUI>();   
        _basicUI = _transitionUI.GetRootUI().GetComponentInChildren<BasicUI>();
        _lobbyInterfaceUI = Instantiate(_lobbyInterfaceUIPref);

        _lobbyInterfaceUI.transform.SetParent(SingletonCanvas._instance.transform, false);
        _lobbyInterfaceUI.transform.SetAsFirstSibling();
        _lobbyControl = _lobbyInterfaceUI.GetComponentInChildren<LobbyControl>();
        _joinStateUI = _lobbyInterfaceUI.transform.Find("JoinStateUI").GetComponent<TextMeshProUGUI>();

        UpdateLobbyStateUI();
        _lobbyControl._onJoined += OnJoined;
        _lobbyControl._onLeaved += OnLeaved;


        //_inputBinding = GetComponent<InputBinding>();   
        //_openLobbyUIKeyBinding = new OpenLobbyUIKeyBinding(_inputBinding, "OpenLobbyUIKeyBinding","l");
        //_inputBinding.EnableMap("OpenLobbyUIKeyBinding");
        //_openLobbyUIKeyBinding._onKeyInputChanged += OnOpenLobbyUIKey;

        SingletonInitialize();
        gameObject.SetActive(true);
    }
    void OnOpenLobbyUIKey(float val)
    {
        if(val == 1)
        {
            _lobbyControl.gameObject.SetActive(!_lobbyControl.gameObject.activeSelf);
        }
    }
    public void JoinLastLobby()
    {
        if (_lastLobby != null)
        {
            var transition = new BasicTransition("JoinLobby", _basicUI, "Joining Lobby...");
            _transitionUI.AddTransition(transition);
            _freeNet._singleLobbyManager.JoinLobby(_lastLobby._lobbyID, (result,lobby) =>
            {
                if(result == Result.Success)
                {
                    OnJoined(lobby);
                }
                else
                {
                    SceneManager.LoadScene("LobbyScene");
                    UpdateLobbyStateUI();
                }
                _basicUI._waitInfoDetail.text = $"{result}";
                _transitionUI.MakeTransitionEnd("JoinLobby");
            });
        }
        else
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
    void UpdateLobbyStateUI()
    {
        if ((_lastLobby!=null) &&_lastLobby._joined)
        {
            if (_lastLobby.GetLobbyCode(out var code))
            {
                _joinStateUI.text = $"Joined Lobby {code}";
            }
        }
        else
        {
            _joinStateUI.text = "No Joined Lobby";
        }
    }
    void OnLeaved(EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        _freeNet._NGOManager.Shutdown();    
        _lastLobby = lobby;
        _lobbyControl.gameObject.SetActive(true);
        UpdateLobbyStateUI();
    }
    void OnJoined(EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        _lastLobby = lobby;
        UpdateLobbyStateUI();
        _lobbyControl.gameObject.SetActive(false);
        if (lobby.GetLobbyCode(out string code))
        {
            _freeNet._NGOManager.OnClientStopped -= NGODisConnected;
            _freeNet._NGOManager.OnClientStarted -= NGOConnected;
            _freeNet._NGOManager.OnClientStopped += NGODisConnected;
            _freeNet._NGOManager.OnClientStarted += NGOConnected;
            var transition = new BasicTransition("NGOClientConnect", _basicUI, "NGO Client Connect...");
            _transitionUI.AddTransition(transition); 
            if (lobby._lobbyOwner.ToString() == lobby._localPUID.ToString())
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartHost(lobby._localPUID.ToString(), code);
            }
            else
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartClient(lobby._localPUID.ToString(), lobby._lobbyOwner.ToString(), code);
            }
        }
    }
    void NGODisConnected(bool b)
    {
        UpdateLobbyStateUI();
        Debug.Log("NGO DisConnected");
        if (_lastLobby != null && _lastLobby._joined)
        {
            var transition = new BasicTransition("LeaveLobby", _basicUI, "Leaving Lobby...");
            _transitionUI.AddTransition(transition);
            _freeNet._singleLobbyManager.LeaveLobby((result, lobby) =>
            {
                if (result == Result.Success)
                {
                    OnLeaved(lobby);
                }
                _basicUI._waitInfoDetail.text = $"{result}";
                _transitionUI.MakeTransitionEnd("LeaveLobby");
            });
        }
    }
    void NGOConnected()
    {
        Debug.Log("NGO Connected");
        _basicUI._waitInfoDetail.text =  "NGO Connect Success";
        _transitionUI.MakeTransitionEnd("NGOClientConnect");

        var transition = new BasicTransition("LoadGame", _basicUI, "Load Game...");
        _transitionUI.AddTransition(transition);
        SceneManager.LoadScene("Lobby");
    }
    void Rlease()
    {
        _openLobbyUIKeyBinding._onKeyInputChanged -= OnOpenLobbyUIKey;
        _lobbyControl._onJoined -= OnJoined;
        _freeNet._NGOManager.OnClientStopped -= NGODisConnected;
        _freeNet._NGOManager.OnClientStarted -= NGOConnected;
    }
}
