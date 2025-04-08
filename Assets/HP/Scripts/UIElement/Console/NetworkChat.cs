using HP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static PlayerMoveControl;

public class NetworkChat : NetworkBehaviour
{
    ConsoleController _controller;
    public override void OnNetworkSpawn()
    {
        _controller = GetComponent<ConsoleController>();
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
