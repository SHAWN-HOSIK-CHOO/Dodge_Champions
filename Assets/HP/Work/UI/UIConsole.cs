using HP;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConsole : MonoBehaviour
{
    [SerializeField]
    TMPInputField _inputField;
    [SerializeField]
    public TMP_InputField _textField;
    [SerializeField]
    Scrollbar _scrollbar;
    
    public event Action<TMPInputField.IInputMode, string> onSubmit;
    private void Awake()
    {
       _inputField.onSubmit.AddListener(OnSubmit);
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
}
