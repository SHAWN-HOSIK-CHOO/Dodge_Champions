using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using GameInput;
using System.Collections;
using UnityEngine;
public class LobbySceneManager : MonoBehaviour
{
    [SerializeField]
    LobbyControl _LobbyControl;
    FreeNet _freeNet;
    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _freeNet = FreeNet._instance;
        _LobbyControl._onJoined += OnJoined;
        _freeNet._singleLobbyManager._onLobbyLeave += onLeaveLobby;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            if(_LobbyControl.gameObject.activeSelf)
            {
                _LobbyControl.gameObject.SetActive(false);
            }
            else
            {
                _LobbyControl.gameObject.SetActive(true);
            }
        }
    }

    void onLeaveLobby(Result result,EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        if(result == Result.Success)
        {
            _LobbyControl.gameObject.SetActive(true);
        }
    }
    void OnJoined(EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        _LobbyControl.gameObject.SetActive(false);
        Debug.Log("연결 시작");
        if (lobby._attribute.TryGetValue("LobbyCode", out var attr))
        {
            string code = attr.Data.Value.Value.AsUtf8;

            if (lobby._lobbyOwner.ToString() == lobby._localPUID.ToString())
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartHost(lobby._localPUID.ToString(), code);
            }
            else
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartClient(lobby._lobbyID.ToString(), lobby._lobbyOwner.ToString(), code);
            }
        }
    }
    private void OnDestroy()
    {
        _LobbyControl._onJoined -= OnJoined;
        _freeNet._singleLobbyManager._onLobbyLeave -= onLeaveLobby;
    }
}
