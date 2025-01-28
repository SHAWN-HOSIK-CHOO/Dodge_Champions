using Epic.OnlineServices.Platform;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FreeNet : SingletonMonoBehaviour<FreeNet>
{
    public EOS_Core _eosCore { get; private set; }
    public EOS_SingleLobbyManager _singleLobbyManager { get; private set; }
    public EOS_LocalUser _localUser { get; private set; }

    public NetworkManager _NGOManager;

    private void Awake()
    {
        SingletonSpawn(this);
    }
    private IEnumerator Start()
    {
        _NGOManager  = GetComponent<NetworkManager>();  
        yield return EOS_Core.WaitInitialize();
        _eosCore = EOS_Core._instance;
        yield return EOS_LocalUser.WaitInitialize();
        _localUser = EOS_LocalUser._instance;
        yield return EOS_SingleLobbyManager.WaitSpawn();
        _singleLobbyManager = EOS_SingleLobbyManager._instance;
        _singleLobbyManager.Init(_eosCore,_localUser);
        yield return EOS_SingleLobbyManager.WaitInitialize();
        SingletonInitialize();
    }

    public void OnDestroy()
    {
    }

    private void OnApplicationQuit()
    {
        try
        {
            _singleLobbyManager.Release();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Release exception: " + ex.Message);
        }
        finally
        {
            _eosCore.Release();
        }
    }
}
