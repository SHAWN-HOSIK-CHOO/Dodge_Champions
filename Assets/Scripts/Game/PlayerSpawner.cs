using GameLobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GameUI;
using System.Collections.Generic; 

namespace Game
{
    public class PlayerSpawner : NetworkBehaviour
    {
        public Transform spawnPoint1;
        public Transform spawnPoint2;

        private bool _isSpawned = false;
        private int _clientsInScene = 0;

        private void OnEnable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            }
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            }
        }

        private void OnLoadEventCompleted(
            string sceneName,
            LoadSceneMode loadSceneMode,
            List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut)
        {
            if (!IsServer) return;

            if (sceneName != SceneManager.GetActiveScene().name)
            {
                return;
            }

            _clientsInScene += clientsCompleted.Count;

            foreach (var clientId in clientsCompleted)
            {
                Debug.Log($"Client {clientId} finished loading {sceneName}");
            }

            if (_clientsInScene == NetworkManager.Singleton.ConnectedClientsIds.Count)
            {
                StartCoroutine(WaitAndSpawnPlayers());
            }
        }

        private IEnumerator WaitAndSpawnPlayers()
        {
            // UIManager가 null이거나 아직 활성화되지 않았다면 기다린다
            while (UIManager.Instance == null || !UIManager.Instance.IsInitialized)
            {
                yield return null;
            }
            
            SpawnPlayers();
            
            GameManager.Instance.StartGameServerRPC();
        }

        private void SpawnPlayers()
        {
            if (_isSpawned) return;
            _isSpawned = true;

            SpawnPlayer(spawnPoint1.position, spawnPoint1.rotation, 0);
            SpawnPlayer(spawnPoint2.position, spawnPoint2.rotation, 1);
        }

        private void SpawnPlayer(Vector3 position, Quaternion rotation, int playerIndex)
        {
            GameObject pfPlayer = PlayerSelectionManager.Instance.confirmedCharacterSOs[playerIndex].pfCharacter;

            GameObject playerInstance = Instantiate(pfPlayer, position, rotation);

            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                ulong clientId = NetworkManager.Singleton.ConnectedClientsList[playerIndex].ClientId;
                networkObject.SpawnWithOwnership(clientId);
            }
        }
    }
}
