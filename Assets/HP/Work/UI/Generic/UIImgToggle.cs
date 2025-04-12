
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIImgToggle : UIToggle
{
    [SerializeField]
    Color _toggleActive;
    Color _cahsedNormalColor;
    [SerializeField]
    Sprite _imageHandleOff;

    [SerializeField]
    Sprite _imageHandleOn;

    protected override void Awake()
    {
        _cahsedNormalColor = _normalColor;
        base.Awake();
    }

    protected override void OnToggle(bool b)
    {
        if (!IsActivated) return;
        _isOn = b;
        if (IsOn)
        {
            _image.sprite = _imageHandleOn;
            _normalColor = _toggleActive;
            GenerateColors();
            _image.DOColor(_normalColor, 0.2f);
        }
        else
        {
            _image.sprite = _imageHandleOff;
            _normalColor = _cahsedNormalColor;
            GenerateColors();
            _image.DOColor(_normalColor, 0.2f);
        }
    }
}
