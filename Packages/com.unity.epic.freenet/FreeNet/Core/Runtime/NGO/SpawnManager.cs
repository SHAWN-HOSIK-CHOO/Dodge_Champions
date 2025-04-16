using UnityEngine;
using static NetworkSpawner;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    Transform _spawnPointB;
    void Start()
    {
        if (NgoManager.Instance._ngoReady)
        {
            OnNgoManagerReady();
        }
        else
        {
            NgoManager.Instance._onNgoManagerReady += OnNgoManagerReady;
        }
    }

    void OnNgoManagerReady()
    {
        if (NgoManager.Instance.IsServer)
        {
            NgoManager.Instance._networkSpawner.Spawn(new SpawnParams()
            {
                pos = _spawnPointB.position,
                rot = _spawnPointB.rotation,
                prefabListName = "DummyPrefab",
                prefabName = "NetworkConsoleUI",
                destroyWithScene = true
            });

            NgoManager.Instance._networkSpawner.Spawn(new SpawnParams()
            {
                pos = _spawnPointB.position,
                rot = _spawnPointB.rotation,
                prefabListName = "DummyPrefab",
                prefabName = "Player",
                destroyWithScene = true
            });

        }
        else
        {
            NgoManager.Instance._networkSpawner.SpawnObjectRpc(true, new SpawnParams()
            {
                pos = _spawnPointB.position,
                rot = _spawnPointB.rotation,
                prefabListName = "DummyPrefab",
                prefabName = "Player",
                destroyWithScene = true
            });
        }
    }
}
