using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    public struct SpawnParams : INetworkSerializable
    {
        public Vector3 pos;
        public bool destroyWithScene;
        public Quaternion rot;
        public string prefabListName;
        public string prefabName;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref pos);
            serializer.SerializeValue(ref destroyWithScene);
            serializer.SerializeValue(ref rot);
            serializer.SerializeValue(ref prefabListName);
            serializer.SerializeValue(ref prefabName);
        }
    }
    Dictionary<string, Dictionary<string, NetworkPrefab>> _prefabs;

    private void Awake()
    {
        _prefabs = new Dictionary<string, Dictionary<string, NetworkPrefab>>();
    }
    void UpdatePrefabList()
    {
        _prefabs = new Dictionary<string, Dictionary<string, NetworkPrefab>>();
        List<NetworkPrefabsList> list = NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists;
        foreach (var networkPrefabList in list)
        {
            var dict = new Dictionary<string, NetworkPrefab>();
            _prefabs.Add(networkPrefabList.name, dict);
            foreach (var networkPrefab in networkPrefabList.PrefabList)
            {
                dict.Add(networkPrefab.Prefab.name, networkPrefab);
            }
        }
    }
    public override void OnNetworkSpawn()
    {
        UpdatePrefabList();
        var ngoManager = (NetworkManager as NgoManager);
        ngoManager._networkSpawner = this;
        ngoManager._onSpawnerSpawned?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("Spawner Despawned");
    }

    public bool GetNetworkPref(string prefabListName, string prefabName, out NetworkPrefab netPrefab)
    {
        netPrefab = null;
        if (_prefabs.TryGetValue(prefabListName, out var prefabList))
        {
            if (prefabList.TryGetValue(prefabName, out netPrefab))
            {
                return true;
            }
        }
        return false;
    }
    public NetworkObject Spawn(SpawnParams param, bool transferOwnership = false, ulong clientID = 0)
    {
        if (IsServer && GetNetworkPref(param.prefabListName, param.prefabName, out var netpref))
        {
            GameObject playerInstance = Instantiate(netpref.Prefab, param.pos, param.rot);
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            if (transferOwnership)
            {
                //networkObject.SpawnWithObservers = false;
                networkObject.SpawnWithOwnership(clientID, param.destroyWithScene);
            }
            else
            {
                //networkObject.SpawnWithObservers = false;
                networkObject.Spawn(param.destroyWithScene);
            }
            return networkObject;
        }
        return null;
    }
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnObjectRpc(bool transferOwnership, SpawnParams param, RpcParams rpcParams = default)
    {
        Spawn(param, transferOwnership, rpcParams.Receive.SenderClientId);
    }
}