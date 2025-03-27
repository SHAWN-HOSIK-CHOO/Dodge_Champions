using Unity.Netcode;
using UnityEngine;

namespace SinglePlayer
{
    public class SinglePlayerSpawner : NetworkBehaviour
    {
        public GameObject pfPlayer;

        private GameObject _spawnedPlayer = null;

        public GameObject SpawnLocalPlayer(Transform spawnTransform)
        {
            if (IsHost)
            {
                Debug.Log("IsHost is true, spawning local player.");
                if (pfPlayer == null)
                {
                    Debug.LogError("pfPlayer is not assigned!");
                    return null;
                }

                _spawnedPlayer = Instantiate(pfPlayer, spawnTransform.position, spawnTransform.rotation);
                if (_spawnedPlayer == null)
                {
                    Debug.LogError("Failed to instantiate player prefab.");
                    return null;
                }

                NetworkObject networkObject = _spawnedPlayer.GetComponent<NetworkObject>();
                if (networkObject == null)
                {
                    Debug.LogError("NetworkObject component is missing on the player prefab!");
                    return null;
                }

                networkObject.SpawnWithOwnership(0);
                Debug.Log("Player spawned successfully.");
                return _spawnedPlayer;
            }
            else
            {
                Debug.Log("IsHost is false, skipping player spawn.");
                return null;
            }
        }

    }
}
