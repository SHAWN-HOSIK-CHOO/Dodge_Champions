using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionSetupExtensions;

public static class InputActionAssetHelper 
{
    public static ControlSchemeSyntax CreateControlScheme(InputActionAsset asset, string schemeName)
    {
        return asset.AddControlScheme(schemeName);
    }
    public static ControlSchemeSyntax AddControlScheme(ControlSchemeSyntax syntax, InputSystemNaming.Device device)
    {
        return syntax.WithRequiredDevice(device.ToInputSystemName());
    }
    public static InputActionMap CreateActionMap(InputActionAsset asset, string actionMap)
    {
        return asset.AddActionMap(actionMap);
    }
    public static BindingSyntax AddKeyboardBinding(InputAction action, Key key)
    {
        string path = InputSystemNaming.Device.Keyboard.ToInputSystemName() + '/' + key;
        return action.AddBinding(path);
    }
    public static BindingSyntax AddMouseBinding(InputAction action, InputSystemNaming.MouseType type)
    {
        string path = InputSystemNaming.Device.Mouse.ToInputSystemName() + '/' + type.ToInputSystemName();
        return action.AddBinding(path);
    }
    public static CompositeSyntax AddCompositeBinding(InputAction action, InputSystemNaming.CompositeType type)
    {
        return action.AddCompositeBinding(type.ToInputSystemName());
    }
    public static CompositeSyntax AddCompositeBinding(CompositeSyntax compositeSyntax, InputSystemNaming.Vector2DSyntax syntax, InputSystemNaming.Device device, Key key)
    {
        string path = device.ToInputSystemName() + '/' + key;
        return compositeSyntax.With(syntax.ToInputSystemName(), path);
    }
}
