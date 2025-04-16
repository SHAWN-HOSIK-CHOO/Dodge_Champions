using UnityEngine;

public partial class EOS_Core : MonoBehaviour
{
    ulong NotifyLoginStatusChangedHandle;
    void InitLogin()
    {
        NotifyLoginStatusChangedHandle = EOSWrapper.LoginControl.AddNotifyLoginStatusChangedOptions(_IAuth, (ref Epic.OnlineServices.Auth.LoginStatusChangedCallbackInfo info) =>
        {
            Debug.Log($"Login Status {info.PrevStatus}=>{info.CurrentStatus}");
        });

    }

    void ReleaseLogin()
    {
        _IAuth.RemoveNotifyLoginStatusChanged(NotifyLoginStatusChangedHandle);
    }
}
