using System.Collections;
using UnityEngine;
public class FreeNet : SingletonMonoBehaviour<FreeNet>
{
    public EOS_Core _eosCore { get; private set; }
    public EOS_LocalUser _localUser { get; private set; }
    public NgoManager _ngoManager { get; private set; }

    private void Awake()
    {
        SingletonSpawn(this);
    }
    private void Start()
    {
        _ngoManager = GetComponent<NgoManager>();
        _localUser = GetComponent<EOS_LocalUser>();
        _eosCore = GetComponent<EOS_Core>();
        _ngoManager.Init(this);
        _eosCore.Run();
        SingletonInitialize();
    }
}
