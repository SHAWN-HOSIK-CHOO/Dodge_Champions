using UnityEngine;
using UnityEngine.InputSystem;

public class UIKeyEvent : UIElement
{
    const string _inputActionAssetName = "UI";
    const string _inputActionMapName = "UIKeyEvent";
    protected InputActionMap _inputActionMap;
    protected override void Awake()
    {
        base.Awake();
    }

    protected void Create()
    {
        _inputActionAsset = InputActionAssetHelper.CreateInputActionAsset(_inputActionAssetName);
        var controlSyntax = InputActionAssetHelper.CreateControlScheme(_inputActionAsset, "keyBoard");
        InputActionAssetHelper.AddControlScheme(controlSyntax, InputSystemNaming.Device.Keyboard);
        _inputActionMap = InputActionAssetHelper.CreateActionMap(_inputActionAsset, _inputActionMapName);
        _inputActionAsset.Enable();
    }

    private void OnDestroy()
    {
        InputActionAssetHelper.ReleaseInputActionAsset(_inputActionAssetName);
    }
}
