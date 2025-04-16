using TMPro;
using UnityEngine;

public class ResolutionSelect : Resolution
{
    [SerializeField]
    TMP_Dropdown _resolutionDropdown;
    [SerializeField]
    UIImgToggle _toggle;
    int _curindex;

    void Start()
    {
        _resolutionDropdown.options.Clear();
        _curindex = -1;
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

    public void ChangeResolution()
    {
        if (_curindex == -1) return;
        var selectedResolution = Screen.resolutions[_curindex];
        base.ChangeResolution(selectedResolution.width, selectedResolution.height, !_toggle.IsOn);
    }
    public void DropboxOptionChange(int index)
    {
        _curindex = index;
    }
}
