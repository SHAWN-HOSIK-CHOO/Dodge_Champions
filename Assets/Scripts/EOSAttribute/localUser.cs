using UnityEngine;

public class localUser : SingletonMonoBehaviour<localUser>
{
    public string _locaPUID;
    private void Awake()
    {
        if ((this as Singleton<localUser>).Singleton(this))
        {

        }
        else
        {
            Destroy(this);
        }
    }

    public void SetlocaPUID(string localPUID)
    {
        _locaPUID = localPUID;
    }
}
