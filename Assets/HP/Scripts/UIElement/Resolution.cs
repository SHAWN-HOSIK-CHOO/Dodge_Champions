using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resolution : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown _resolutionDropdown;
    [SerializeField]
    Toggle _toggle;
    [SerializeField]
    Button _button;
    int _curindex;
    void Start()
    {
        _button.onClick.AddListener(Button);
        _resolutionDropdown.options.Clear();
        _curindex = 0;
        int optionNum = 0;
        _resolutionDropdown.onValueChanged.AddListener(DropboxOptionChange);
        foreach (var item in Screen.resolutions)
        {
            var option = new TMP_Dropdown.OptionData();
            option.text = item.width + " x " + item.height;
            _resolutionDropdown.options.Add(option);
            if (item.width == Screen.width && item.height == Screen.height)
            {
                _resolutionDropdown.value = optionNum;
            }
            optionNum++;
        }
    }

    void Button()
    {
        var selectedResolution = Screen.resolutions[_curindex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, !_toggle.isOn);
    }

    public void DropboxOptionChange(int index)
    {
        _curindex = index;
    }
}
