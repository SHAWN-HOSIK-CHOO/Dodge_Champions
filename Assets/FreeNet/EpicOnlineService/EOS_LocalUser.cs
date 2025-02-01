using Epic.OnlineServices;
using UnityEngine;

public class EOS_LocalUser : SingletonMonoBehaviour<EOS_LocalUser>
{
    public string _locaPUID;
    private void Awake()
    {
        if (SingletonSpawn(this))
        {
            SingletonInitialize();
        }
    }

    public ProductUserId GetLocalPUID()
    {
        return ProductUserId.FromString(_locaPUID);
    }
    public void SetlocaPUID(string localPUID)
    {
        _locaPUID = localPUID;
    }
}
