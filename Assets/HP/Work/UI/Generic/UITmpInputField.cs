using HP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UITmpInputField : UIKeyEvent
{
    [SerializeField]
    TMPInputField _inputField;
    InputAction _inputAction;
    [SerializeField]
    bool _useInputMode;
    [SerializeField]
    GameObject _mode;
    public class Mode : TMPInputField.IInputMode
    {
        public enum InputMode
        {
            T, // Normal Text Mode
            S, // System command Mode
        }
        public InputMode _mode;

        void TMPInputField.IInputMode.ChangeModeNext()
        {
            _mode = (InputMode)(((int)_mode + 1) % System.Enum.GetValues(typeof(InputMode)).Length);
        }

        string TMPInputField.IInputMode.GetName()
        {
            return _mode.ToString();
        }
    }

    protected override void Awake()
    {
        Create();
        _inputAction = InputActionAssetHelper.AddAction(_inputActionMap, "Enter", InputActionType.Value);
        InputActionAssetHelper.AddKeyboardBinding(_inputAction, Key.Enter);
        _inputAction.performed += OnEnter;
        _inputField.InputMode = new Mode();
        _mode.SetActive(_useInputMode);
        base.Awake();
    }
    public override void OnActivate()
    {
        _inputAction.Enable();
    }
    public override void OnDeActivate()
    {
        _inputAction.Disable();
    }
    private void OnEnter(InputAction.CallbackContext ctx)
    {
        if (!IsActivated) return;
        bool b = ctx.ReadValue<float>() == 1;
        if (EventSystem.current != null)
        {
            var obj = EventSystem.current.currentSelectedGameObject;
            if (obj != null && obj.GetComponent<TMPInputField>() != null) return;
            if (b && !_inputField.isFocused && _inputField._useAutoFocus)
            {
                _inputField.ActivateInputField();
            }
        }
    }

    public string text => _inputField.text;
}
