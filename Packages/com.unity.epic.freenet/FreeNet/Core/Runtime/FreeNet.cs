using UnityEngine;

public class FreeNet : SingletonMonoBehaviour<FreeNet>
{
    [SerializeField]
    public EOS_Core _eosCore;
    [SerializeField]
    public NgoManager _ngoManager;
    public EOS_LocalUser _localUser { get; private set; }
    private void Awake()
    {
        SingletonSpawn(this);
    }
    private void Start()
    {
        _ngoManager.Init(this);
        _localUser = new EOS_LocalUser();
        _eosCore.Run();
        SingletonInitialize();
    }

}
