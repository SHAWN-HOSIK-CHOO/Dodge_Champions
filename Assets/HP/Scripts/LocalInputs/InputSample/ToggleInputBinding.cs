using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HP
{
    public class ToggleInputBinding
    {
        InputActionAsset _inputActionAsset;
        InputActionMap _inputActionMap;
        InputAction _toggleAction;
        public float _toggleInput { get; private set; }
        public event Action<bool> _onToggleInputChanged;
        public ToggleInputBinding(Key toggleKey)
        {
            _inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            var controlSyntax = InputActionAssetHelper.CreateControlScheme(_inputActionAsset, "Keyboard");
            InputActionAssetHelper.AddControlScheme(controlSyntax, InputSystemNaming.Device.Keyboard);
            _inputActionMap = InputActionAssetHelper.CreateActionMap(_inputActionAsset, "ToggleInputBinding");
            _toggleAction = _inputActionMap.AddAction("Toggle", InputActionType.Button);
            InputActionAssetHelper.AddKeyboardBinding(_toggleAction, toggleKey);
            _toggleAction.performed += OnToggle;
        }

        public void Enable(bool b)
        {
            if(b)
            {
                _inputActionAsset.Enable();
            }
            else
            {
                _inputActionAsset.Disable();
            }
        }

        public void Dispose()
        {
            GameObject.Destroy(_inputActionAsset);
        }

        private void OnToggle(InputAction.CallbackContext ctx)
        {
            _toggleInput = ctx.ReadValue<float>();
            _onToggleInputChanged?.Invoke(_toggleInput == 1);
        }
    }
}