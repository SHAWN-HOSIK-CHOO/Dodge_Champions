using System;
using UnityEngine;
using static InputSystemNaming;
using UnityEngine.InputSystem;
using HP;

namespace HP
{
    public class PlayerInput
    {
        string _inputActionAssetName;
        public InputActionAsset _inputActionAsset;
        public InputActionMap _inputActionMap;
        public InputAction _WASDAction;
        public InputAction _mouseAction;
        public InputAction _jumpAction;

        public Vector3 _moveInput { get; private set; }
        public Vector2 _mouseInput { get; private set; }
        public float _jumpInput { get; private set; }


        public event Action<Vector3> _onMoveInputChanged;
        public event Action<Vector2> _onMouseInputChanged;
        public event Action<float> _onJumpInputChanged;

        public PlayerInput(string inputActionAssetName)
        {
            _inputActionAssetName = inputActionAssetName;
            _inputActionAsset = InputActionAssetHelper.CreateInputActionAsset(_inputActionAssetName);
            var controlSyntax = InputActionAssetHelper.CreateControlScheme(_inputActionAsset, "KeyboardMouse");
            InputActionAssetHelper.AddControlScheme(controlSyntax, InputSystemNaming.Device.Keyboard);
            InputActionAssetHelper.AddControlScheme(controlSyntax, InputSystemNaming.Device.Mouse);

            _inputActionMap = InputActionAssetHelper.CreateActionMap(_inputActionAsset, "WASD_MouseBinding");
            _WASDAction = _inputActionMap.AddAction("WASD", InputActionType.PassThrough);
            var compositeSyntax = InputActionAssetHelper.AddCompositeBinding(_WASDAction, CompositeType.Vector2D);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Up, Device.Keyboard, Key.W);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Down, Device.Keyboard, Key.S);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Left, Device.Keyboard, Key.A);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Right, Device.Keyboard, Key.D);
            _WASDAction.performed += OnWASD;

            _jumpAction = _inputActionMap.AddAction("Jump", InputActionType.Value);
            InputActionAssetHelper.AddKeyboardBinding(_jumpAction, Key.Space);
            _jumpAction.performed += OnJump;


            _mouseAction = _inputActionMap.AddAction("MouseMove", InputActionType.Value);
            var bindsyntax = InputActionAssetHelper.AddMouseBinding(_mouseAction, MouseType.delta);
            bindsyntax.WithProcessor(InputSystemNaming.Processor.ScaleVector2.ToInputSystemName(new Vector2(0.5f, 0.5f)));
            bindsyntax.WithProcessor(InputSystemNaming.Processor.Invert.ToInputSystemName());
            _mouseAction.performed += OnMouse;
            _inputActionAssetName = inputActionAssetName;
        }
        public void Enable(bool b)
        {
            if (b)
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
            InputActionAssetHelper.ReleaseInputActionAsset(_inputActionAssetName);
        }
        private void OnWASD(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
            _onMoveInputChanged?.Invoke(_moveInput);
        }
        private void OnMouse(InputAction.CallbackContext ctx)
        {
            _mouseInput = ctx.ReadValue<Vector2>();
            _onMouseInputChanged?.Invoke(_mouseInput);
        }
        private void OnJump(InputAction.CallbackContext ctx)
        {
            _jumpInput = ctx.ReadValue<float>();
            _onJumpInputChanged?.Invoke(_jumpInput);
        }
    }
}