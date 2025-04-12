using HP;
using Unity.Netcode;

public class NetworkChat : NetworkBehaviour
{
    UIConsole _controller;
    public override void OnNetworkSpawn()
    {
        _controller = GetComponent<UIConsole>();
        _controller.onSubmit += OnSubmit;
    }

    void OnSubmit(TMPInputField.IInputMode mode, string text)
    {
        SendChatRpc(text);
    }

    [Rpc(SendTo.NotMe)]
    void SendChatRpc(string text)
    {
        _controller.AddText(text);
    }
}
