using System;
using UnityEngine.InputSystem;

namespace HP
{
    public class ToggleInputBinding
    {
        string _actionMap;
        InputManager _inputManager;
        InputActionMap _inputActionMap;
        InputAction _inputAction;
        public float _toggleInput { get; private set; }
        public event Action<bool> _onToggleInputChanged;
        public ToggleInputBinding(InputManager inputManager, InputActionMap actionMap, Key toggleKey)
        {
            _inputManager = inputManager;
            _inputActionMap = actionMap;
            _inputAction = _inputManager.AddAction(_inputActionMap, "Toggle", InputActionType.Button);
            _inputManager.AddKeyboardBinding(_inputAction, toggleKey);
            _inputManager.AddCallback(_inputAction, InputManager.CallbackType.Performed, OnToggle);
        }

        public void Dispose()
        {
            _inputManager.RemoveAction(_inputAction);
            _inputManager.RemoveCallback(_inputAction, InputManager.CallbackType.Performed, OnToggle);
        }

        private void OnToggle(InputAction.CallbackContext ctx)
        {
            _toggleInput = ctx.ReadValue<float>();
            _onToggleInputChanged?.Invoke(_toggleInput == 1);
        }
    }
}