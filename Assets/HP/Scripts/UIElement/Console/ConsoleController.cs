using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using HP;
using System;
using static System.Net.Mime.MediaTypeNames;
public class ConsoleController : MonoBehaviour {

    [SerializeField]
    GameObject _inputFieldObject;
    [SerializeField]
    CutomTMPInputField _inputField;
    [SerializeField]
    public TMP_InputField _textField;
    [SerializeField]
    Scrollbar _scrollbar;
    [SerializeField]
    bool _showInputField;

    public event Action<CutomTMPInputField.InputMode, string> onSubmit;

    private void Awake()
    {
        _inputField.onSubmit.AddListener(OnSubmit);
       
    }
    private void Start()
    {
        _inputFieldObject.SetActive(_showInputField);
    }
    public void ShowInputField(bool b)
    {
        _showInputField = b;
        _inputFieldObject.SetActive(b);
    }

    private void OnEnable()
    {
        ShowInputField(_showInputField);
    }

    public void AddText(string text, bool scroll = true, bool newLine = true)
    {
        if (newLine) _textField.text += "\n";

        if(_inputField._InputMode == CutomTMPInputField.InputMode.S)
        {
            _textField.text +=$"<color=yellow>{text}</color>";
        }
        else
        {
            _textField.text += text;
        }
        if (scroll) _scrollbar.value = 1f;
    }

    void OnSubmit(string newText)
    {
        if (newText != string.Empty)
        {
            AddText(newText);
            onSubmit?.Invoke(_inputField._InputMode, newText);
        }
    }

    private void OnDestroy()
    {
        _inputField.onSubmit.RemoveListener(OnSubmit);
    }
}
