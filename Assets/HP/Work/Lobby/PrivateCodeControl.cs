using HP;
using System;
using UnityEngine;

public class PrivateCodeControl : MonoBehaviour
{
    [SerializeField]
    public TMPInputField _inputField;
    LobbyElement _lobbyInfoElement;

    public event Action<string, LobbyElement> _onJoinRequest;
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
    public void ActivateInpuField(LobbyElement element)
    {
        _lobbyInfoElement = element;
        _inputField.text = string.Empty;
        gameObject.SetActive(true);
    }


    void OnSubmit(string text)
    {
        _onJoinRequest?.Invoke(text, _lobbyInfoElement);
    }
    private void OnDestroy()
    {
        _inputField.onSubmit.RemoveListener(OnSubmit);
    }
}
