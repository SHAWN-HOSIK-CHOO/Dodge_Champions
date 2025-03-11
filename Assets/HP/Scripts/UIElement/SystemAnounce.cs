using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemAnounce : MonoBehaviour
{
    [SerializeField]
    TMP_Text _message;
    [SerializeField]
    Button _closeButton;

    private void Start()
    {
        _closeButton.onClick.AddListener(OnClickCloseButton);
    }
    public void OnClickCloseButton()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    public void Show(string message)
    {
        _message.text = message;
        gameObject.SetActive(true);
    }
}
