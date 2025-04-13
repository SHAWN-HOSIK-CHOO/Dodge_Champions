using UnityEngine;
using static NetworkSpawner;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    Transform _spawnPointB;
    void Start()
    {
        if(NgoManager.Singleton._ngoReady)
        {
            OnNgoManagerReady();
        }
        else
        {
            NgoManager.Singleton._onNgoManagerReady += OnNgoManagerReady;
        }
    }

    void OnNgoManagerReady()
    {
        if (NgoManager.Singleton.IsServer)
        {
            NgoManager.Singleton._networkSpawner.Spawn(new SpawnParams()
            {
                pos = _spawnPointB.position,
                rot = _spawnPointB.rotation,
                prefabListName = "DummyPrefab",
                prefabName = "NetworkConsoleUI",
                destroyWithScene = true
            });

            NgoManager.Singleton._networkSpawner.Spawn(new SpawnParams()
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
            NgoManager.Singleton._networkSpawner.SpawnObjectRpc(true, new SpawnParams()
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
