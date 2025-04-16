using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;
namespace HP
{
    public class CustomNetworkManager : NgoManager
    {
        public static new CustomNetworkManager Instance => (NetworkManager.Singleton as CustomNetworkManager);
        EOS_Lobby _currentLobby;
        public event Action<EOS_Lobby> OnJoinLobby;
        private void Start()
        {
            _onNgoManagerReady += OnNgoReady;
        }
        void OnNgoReady()
        {
            OnJoinLobby?.Invoke(_currentLobby);
            if (IsServer)
                SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }

        public void JoinLobby(EOS_Lobby lobby)
        {
            _currentLobby = lobby;
            if (lobby._attribute.GetLobbySocket(out var socketName))
            {
                if (lobby._lobbyOwner == FreeNet.Instance._localUser._localPUID)
                {
                    StartHost(FreeNet.Instance._localUser._localPUID, socketName);
                }
                else
                {
                    StartClient(FreeNet.Instance._localUser._localPUID, lobby._lobbyOwner, socketName);
                }
            }
        }
    }
}