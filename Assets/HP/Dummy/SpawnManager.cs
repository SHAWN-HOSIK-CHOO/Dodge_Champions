using UnityEngine;
using static NetworkSpawner;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    TimeSystemGUI timesystemGUI;
    [SerializeField]
    Transform _spawnPointB;
    [SerializeField]
    NgoManager _ngoManager;
    void Start()
    {
        _ngoManager._onNgoManagerReady += OnNgoManagerReady;
    }

    void OnNgoManagerReady()
    {
        timesystemGUI.Init();
        if (_ngoManager.IsServer)
        {
            _ngoManager._networkSpawner.Spawn(new SpawnParams()
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
            _ngoManager._networkSpawner.SpawnObjectRpc(true, new SpawnParams()
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
