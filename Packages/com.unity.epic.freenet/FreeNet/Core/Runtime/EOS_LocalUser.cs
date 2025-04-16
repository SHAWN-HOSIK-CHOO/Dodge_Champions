using Epic.OnlineServices.UserInfo;

public class EOS_LocalUser
{
    public UserInfoData? _localUserInfo;
    public EOSWrapper.ETC.EAID _localEAID;
    public EOSWrapper.ETC.PUID _localPUID;
    private string localEAID => _localEAID._eaid;
    private string localPUID => _localPUID._puid;
}
