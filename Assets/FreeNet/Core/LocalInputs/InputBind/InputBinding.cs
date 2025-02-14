using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBinding : MonoBehaviour
{
    PlayerInput _playerInput;
    InputActionAsset _inputAsset;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        if (!_playerInput.user.valid)
        {
            Debug.LogWarning("PlayerInput user is not valid. Skipping control scheme switch.");
            return;
        }

        _inputAsset = _playerInput.actions;

        AddKeybouadMouseScheme();

        _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
    }


    void AddKeybouadMouseScheme()
    {
        var keyboardMouseScheme = new InputControlScheme("Keyboard&Mouse")
            .WithRequiredDevice("<Keyboard>")
            .WithRequiredDevice("<Mouse>");

        if (!_playerInput.actions.controlSchemes.Contains(keyboardMouseScheme))
        {
            _playerInput.actions.AddControlScheme(keyboardMouseScheme);
        }

        // 현재 컨트롤 스킴을 "Keyboard&Mouse"로 변경
        if (_playerInput.currentControlScheme != "Keyboard&Mouse")
        {
            _playerInput.SwitchCurrentControlScheme("Keyboard&Mouse");
        }
    }
    public void EnableMap(string name)
    {
        if (FindActionMap(name, out var map))
        {
            map.Enable();
        }
    }
    public void AddActionMap(InputActionMap actionMap)
    {
        _playerInput.actions.AddActionMap(actionMap);
        _playerInput.SwitchCurrentActionMap(actionMap.name);
    }
    public void RemoveActionMap(InputActionMap actionMap)
    {
        _playerInput.actions.RemoveActionMap(actionMap);
    }
    public bool FindActionMap(string name,out InputActionMap map)
    {
        map = _playerInput.actions.FindActionMap(name);
        return map != null;
    }

    private void OnDestroy()
    {
        Destroy(_inputAsset);
    }
}