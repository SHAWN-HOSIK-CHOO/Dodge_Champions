using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CreateControl : MonoBehaviour
{
    [SerializeField]
    public HP.CutomTMPInputField _roomNameInputField;
    [SerializeField]
    public Toggle _isPrivate;
    [SerializeField]
    public TMP_InputField _roomCodeInputField;

    [SerializeField]
    public Button _submitButton;
    [SerializeField]
    public Button _cancelButton;

    [SerializeField]
    GameObject _ModeElementParent;
    [SerializeField]
    GameObject _modeElementPrf;

    [HideInInspector]
    public ModeElement curSelectedMode;

    private void Start()
    {
        AddMode("World1").Select();
        AddMode("World2");
        AddMode("World3");
    }

    public void SelecteMode(ModeElement element)
    {
        if(curSelectedMode != null)
        {
            curSelectedMode.UnSelect();
        }
        curSelectedMode = element;
    }

    ModeElement AddMode(string mode)
    {
        var item = Instantiate(_modeElementPrf).GetComponent<ModeElement>();
        item.transform.SetParent(_ModeElementParent.transform);
        item.Init(this,mode);
        return item;
    }
}
