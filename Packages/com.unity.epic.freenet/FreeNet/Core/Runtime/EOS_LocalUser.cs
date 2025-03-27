using Epic.OnlineServices.UserInfo;
using UnityEngine;

public class EOS_LocalUser : MonoBehaviour
{
    public UserInfoData? _localUserInfo;
    public EOSWrapper.ETC.EAID _localEAID;
    public EOSWrapper.ETC.PUID _localPUID;
    [SerializeField]
    private string localEAID => _localEAID._eaid;
    [SerializeField]
    private string localPUID => _localPUID._puid;
}
