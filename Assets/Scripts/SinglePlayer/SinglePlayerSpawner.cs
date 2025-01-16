using System;
using Unity.Netcode;
using UnityEngine;

namespace SinglePlayer
{
    public class SinglePlayerSpawner : NetworkBehaviour
    {
        public GameObject pfPlayer;
        public Transform  spawnTransform;

        private GameObject _spawnedPlayer = null;

        public override void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn Called");
            //NetworkManager.Singleton.StartHost();
            SpawnLocalPlayer();
            if (_spawnedPlayer != null)
            {
                SinglePlayerGM.Instance.Initialize(_spawnedPlayer);
            }
            else
            {
                Debug.LogError("SpawnLocalPlayer did not spawn the player!");
            }
        }

        private void SpawnLocalPlayer()
        {
            if (IsHost)
            {
                Debug.Log("IsHost is true, spawning local player.");
                if (pfPlayer == null)
                {
                    Debug.LogError("pfPlayer is not assigned!");
                    return;
                }

                _spawnedPlayer = Instantiate(pfPlayer, spawnTransform.position, spawnTransform.rotation);
                if (_spawnedPlayer == null)
                {
                    Debug.LogError("Failed to instantiate player prefab.");
                    return;
                }

                NetworkObject networkObject = _spawnedPlayer.GetComponent<NetworkObject>();
                if (networkObject == null)
                {
                    Debug.LogError("NetworkObject component is missing on the player prefab!");
                    return;
                }

                networkObject.SpawnWithOwnership(0);
                Debug.Log("Player spawned successfully.");
            }
            else
            {
                Debug.Log("IsHost is false, skipping player spawn.");
            }
        }

    }
}
