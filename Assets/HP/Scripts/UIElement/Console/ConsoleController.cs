using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using HP;
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
    private void Awake()
    {
        _inputField.onSubmit.AddListener(OnSubmit);
        ShowInputField(_showInputField);
    }

    public void ShowInputField(bool b)
    {
        _inputFieldObject.SetActive(b);
    }

    public void AddText(string text, bool scroll = true, bool newLine = true)
    {
        if (newLine) _textField.text += "\n";
        _textField.text += text;
        if (scroll) _scrollbar.value = 1f;

        Debug.Log("Add" + text);

    }

    void OnSubmit(string newText)
    {
        if (newText != string.Empty)
        {
            AddText(newText);
        }
    }

    private void OnDestroy()
    {
        _inputField.onSubmit.RemoveListener(OnSubmit);
    }
}
