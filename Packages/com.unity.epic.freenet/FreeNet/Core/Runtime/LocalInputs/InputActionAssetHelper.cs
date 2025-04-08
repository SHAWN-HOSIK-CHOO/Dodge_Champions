using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionSetupExtensions;

public class InputActionAssetHelper
{
    Dictionary<string,(int, InputActionAsset)> _inputActionAssets;
    private static InputActionAssetHelper _singleton;
    public static InputActionAssetHelper Singleton
    {
        get
        {
            if (_singleton == null)
                _singleton = new InputActionAssetHelper();
            return _singleton;
        }
    }
    public InputActionAssetHelper()
    {
        _inputActionAssets = new Dictionary<string, (int,InputActionAsset)>();
        
    }
    public static void ReleaseInputActionAsset(string name)
    {
        if (_singleton._inputActionAssets.TryGetValue(name, out var value))
        {
            value.Item1--;
            if (value.Item1 <= 0)
            {
                _singleton._inputActionAssets.Remove(name);
                GameObject.Destroy(value.Item2);
            }
        }
    }
    public static InputActionAsset CreateInputActionAsset(string name)
    {
        if(Singleton._inputActionAssets.TryGetValue(name,out var value))
        {
            value.Item1++;
            return value.Item2;
        }
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        Singleton._inputActionAssets.Add(name, (1,asset));
        return asset;
    }
    public static ControlSchemeSyntax CreateControlScheme(InputActionAsset asset, string schemeName)
    {
        int index = asset.FindControlSchemeIndex(schemeName);
        if (index !=-1)
        {
            asset.RemoveControlScheme(schemeName);
        }
        return asset.AddControlScheme(schemeName);
    }
    public static ControlSchemeSyntax AddControlScheme(ControlSchemeSyntax syntax, InputSystemNaming.Device device)
    {
        return syntax.WithRequiredDevice(device.ToInputSystemName());
    }
    public static InputActionMap CreateActionMap(InputActionAsset asset, string actionMap)
    {
        var actionmap = asset.FindActionMap(actionMap);
        if(actionmap != null)
        {
            return actionmap;
        }
        return asset.AddActionMap(actionMap);
    }
    public static InputAction AddAction(InputActionMap actionMap, string actionMapName, InputActionType type)
    {
        var action = actionMap.FindAction(actionMapName);
        if (action != null)
        {
            return action;
        }
        return actionMap.AddAction(actionMapName, type);
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
