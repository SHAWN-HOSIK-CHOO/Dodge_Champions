using UnityEngine;

public partial class EOS_Core : MonoBehaviour
{
    ulong NotifyAuthExpirationHandle;
    ulong NotifyConnectStatusChangedHandle;
    void InitConnect()
    {
        NotifyAuthExpirationHandle = EOSWrapper.ConnectControl.AddNotifyAuthExpiration(_IConnect, (ref Epic.OnlineServices.Connect.AuthExpirationCallbackInfo info) =>
        {

            Debug.LogWarning($"AuthExpiration {info.LocalUserId}");
        });
        NotifyConnectStatusChangedHandle = EOSWrapper.ConnectControl.AddNotifyLoginStatusChanged(_IConnect, (ref Epic.OnlineServices.Connect.LoginStatusChangedCallbackInfo info) =>
        {
            Debug.Log($"Connect Status {info.PreviousStatus}=>{info.CurrentStatus}");
        });
    }

    void ReleaseConnect()
    {
        _IConnect.RemoveNotifyLoginStatusChanged(NotifyAuthExpirationHandle);
        _IConnect.RemoveNotifyAuthExpiration(NotifyConnectStatusChangedHandle);
    }
}
