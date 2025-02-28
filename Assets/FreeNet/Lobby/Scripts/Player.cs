using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    LobbySceneManager _lobbySceneManager;

    void OnSpawnerCreated()
    {
        _lobbySceneManager = Object.FindAnyObjectByType<LobbySceneManager>();
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void RequestPUIDRpc()
    {
        
        //_lobbySceneManager.SendPUIDRpc(puid);
    }
}
