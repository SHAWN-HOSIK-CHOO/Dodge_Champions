using HP;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConsoleController : MonoBehaviour
{
    [SerializeField]
    GameObject _inputFieldObject;
    [SerializeField]
    TMPInputField _inputField;
    [SerializeField]
    public TMP_InputField _textField;
    [SerializeField]
    Scrollbar _scrollbar;
    [SerializeField]
    bool _useInputField;
    [SerializeField]
    bool _useInputMode;
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

    public event Action<TMPInputField.IInputMode, string> onSubmit;

    private void Awake()
    {
        //_inputField.onSubmit.AddListener(OnSubmit);

    }
    private void Start()
    {
        _inputFieldObject.SetActive(_useInputField);
        _inputField.InputMode = new Mode();
        //_inputField.ShowInputMode(_useInputMode);
    }
    public void ShowInputField(bool b)
    {
        _useInputField = b;
        _inputFieldObject.SetActive(b);
    }
    private void OnEnable()
    {
        ShowInputField(_useInputField);
    }
    public void AddText(string text, bool scroll = true, bool newLine = true)
    {
        if (newLine) _textField.text += "\n";
        _textField.text += text;
        if (scroll) _scrollbar.value = 1f;
    }
    void OnSubmit(string newText)
    {
        if (newText != string.Empty)
        {
            AddText(newText);
            onSubmit?.Invoke(_inputField.InputMode, newText);
        }
    }
    private void OnDestroy()
    {
       // _inputField.onSubmit.RemoveListener(OnSubmit);
    }
}
