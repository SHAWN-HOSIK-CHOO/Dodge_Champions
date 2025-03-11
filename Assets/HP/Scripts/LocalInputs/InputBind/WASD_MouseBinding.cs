using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystemNaming;
namespace HP
{
    public class WASD_MouseBinding
    {
        InputManager _inputManager;
        InputActionMap _inputActionMap;
        InputAction _keboardAction;
        InputAction _mouseAction;
        Vector3 _moveInput;
        Vector2 _mouseInput;

        public event Action<Vector3> _onMoveInputChanged;
        public event Action<Vector2> _onMouseInputChanged;

        public WASD_MouseBinding(InputManager inputManager, InputActionMap actionMap)
        {
            _inputManager = inputManager;
            _inputActionMap = actionMap;
            _keboardAction = _inputManager.AddAction(_inputActionMap, "WASD", InputActionType.PassThrough);
            var compositeSyntax = _inputManager.AddCompositeBinding(_keboardAction, CompositeType.Vector2D);
            _inputManager.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Up, Device.Keyboard, Key.W);
            _inputManager.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Down, Device.Keyboard, Key.S);
            _inputManager.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Left, Device.Keyboard, Key.A);
            _inputManager.AddCompositeBinding(compositeSyntax, InputSystemNaming.Vector2DSyntax.Right, Device.Keyboard, Key.D);
            _inputManager.AddCallback(_keboardAction, InputManager.CallbackType.Performed, OnWASD);

            _mouseAction = _inputManager.AddAction(_inputActionMap, "MouseMove", InputActionType.Value);
            var syntax = _inputManager.AddMouseBinding(_mouseAction, MouseType.delta);
            syntax.WithProcessor(InputSystemNaming.Processor.ScaleVector2.ToInputSystemName(new Vector2(0.5f, 0.5f)));
            syntax.WithProcessor(InputSystemNaming.Processor.Invert.ToInputSystemName());
            _inputManager.AddCallback(_mouseAction, InputManager.CallbackType.Performed, OnMouse);

        }

        public void Dispose()
        {
            _inputManager.RemoveAction(_keboardAction);
            _inputManager.RemoveCallback(_keboardAction, InputManager.CallbackType.Performed, OnWASD);

            _inputManager.RemoveAction(_mouseAction);
            _inputManager.RemoveCallback(_mouseAction, InputManager.CallbackType.Performed, OnMouse);
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
    }
}