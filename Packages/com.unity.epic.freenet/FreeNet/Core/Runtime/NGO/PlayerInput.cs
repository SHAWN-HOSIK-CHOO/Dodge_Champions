using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystemNaming;

namespace HP
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField]
        string _inputActionAssetName = "PlayerInput";
        public InputActionAsset _inputActionAsset;
        public InputActionMap _inputActionMap;
        public InputAction _WASDAction;
        public InputAction _mouseDeltaAction;
        public InputAction _mouseLeftClickAction;
        public InputAction _mouseRightClickAction;
        public InputAction _jumpAction;

        public Vector3 _moveInput { get; private set; }
        public Vector2 _mouseDeltaInput { get; private set; }
        public float _jumpInput { get; private set; }
        public float _mouseLeftInput { get; private set; }
        public float _mouseRightInput { get; private set; }

        public event Action<Vector3> _onMoveInputChanged;
        public event Action<Vector2> _onMouseDeltaInputChanged;
        public event Action<float> _onJumpInputChanged;
        public event Action<float> _onMouseLeftInputChanged;
        public event Action<float> _onMouseRightInputChanged;


        private void Awake()
        {
            _inputActionAsset = InputActionAssetHelper.CreateInputActionAsset(_inputActionAssetName);
            var controlSyntax = InputActionAssetHelper.CreateControlScheme(_inputActionAsset, "KeyboardMouse");
            InputActionAssetHelper.AddControlScheme(controlSyntax, InputSystemNaming.Device.Keyboard);
            InputActionAssetHelper.AddControlScheme(controlSyntax, InputSystemNaming.Device.Mouse);

            _inputActionMap = InputActionAssetHelper.CreateActionMap(_inputActionAsset, "WASD_MouseBinding");
            _WASDAction = InputActionAssetHelper.AddAction(_inputActionMap, "WASD", InputActionType.PassThrough);
            var compositeSyntax = InputActionAssetHelper.AddCompositeBinding(_WASDAction, CompositeType.Vector2D);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Up, Device.Keyboard, Key.W);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Down, Device.Keyboard, Key.S);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Left, Device.Keyboard, Key.A);
            InputActionAssetHelper.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Right, Device.Keyboard, Key.D);
            _WASDAction.performed += OnWASD;

            _jumpAction = InputActionAssetHelper.AddAction(_inputActionMap, "Jump", InputActionType.Value);
            InputActionAssetHelper.AddKeyboardBinding(_jumpAction, Key.Space);
            _jumpAction.performed += OnJump;


            _mouseDeltaAction = InputActionAssetHelper.AddAction(_inputActionMap, "MouseMove", InputActionType.Value);
            var bindsyntax = InputActionAssetHelper.AddMouseBinding(_mouseDeltaAction, MouseType.delta);
            bindsyntax.WithProcessor(InputSystemNaming.Processor.ScaleVector2.ToInputSystemName(new Vector2(0.5f, 0.5f)));
            bindsyntax.WithProcessor(InputSystemNaming.Processor.Invert.ToInputSystemName());
            _mouseDeltaAction.performed += OnMouseDelta;


            _mouseLeftClickAction = InputActionAssetHelper.AddAction(_inputActionMap, "MouseLeftClick", InputActionType.Button);
            InputActionAssetHelper.AddMouseBinding(_mouseLeftClickAction, MouseType.leftButton);
            _mouseLeftClickAction.performed += OnMouseLeft;

            _mouseRightClickAction = InputActionAssetHelper.AddAction(_inputActionMap, "MouseRightClick", InputActionType.Button);
            InputActionAssetHelper.AddMouseBinding(_mouseRightClickAction, MouseType.rightButton);
            _mouseRightClickAction.performed += OnMouseRight;
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
        private void OnMouseDelta(InputAction.CallbackContext ctx)
        {
            _mouseDeltaInput = ctx.ReadValue<Vector2>();
            _onMouseDeltaInputChanged?.Invoke(_mouseDeltaInput);
        }
        private void OnMouseRight(InputAction.CallbackContext ctx)
        {
            _mouseRightInput = ctx.ReadValue<float>();
            _onMouseRightInputChanged?.Invoke(_mouseRightInput);
        }
        private void OnMouseLeft(InputAction.CallbackContext ctx)
        {
            _mouseLeftInput = ctx.ReadValue<float>();
            _onMouseLeftInputChanged?.Invoke(_mouseLeftInput);
        }
        private void OnJump(InputAction.CallbackContext ctx)
        {
            _jumpInput = ctx.ReadValue<float>();
            _onJumpInputChanged?.Invoke(_jumpInput);
        }
    }
}