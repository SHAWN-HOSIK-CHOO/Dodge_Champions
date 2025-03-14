using HP;
using System;
using UnityEngine;
using UnityEngine.UI;
using static EOSWrapper;

public class PrivateCodeControl : MonoBehaviour
{
    [SerializeField]
    public CutomTMPInputField _inputField;
    LobbyInfoElement _lobbyInfoElement;

    public event Action<string, LobbyInfoElement> _onJoinRequest;
    void Start()
    {
        _inputField.onSubmit.AddListener(OnSubmit);
        _inputField._useAutoFocus = false;
        _inputField._resetOnSubmit = false;
    }

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }
    public void ActivateInpuField(LobbyInfoElement element)
    {
        _lobbyInfoElement = element;
        _inputField.text = string.Empty;
        gameObject.SetActive(true);
    }


    void OnSubmit(string text)
    {
        _onJoinRequest?.Invoke(text,_lobbyInfoElement);
    }
    private void OnDestroy()
    {
        _inputField.onSubmit.RemoveListener(OnSubmit);
    }
}
