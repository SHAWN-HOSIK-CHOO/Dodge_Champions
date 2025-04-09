using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIImgToggle : UIToggle
{
    [SerializeField]
    Image _imageBG;
    [SerializeField]
    Image _imageHandle;

    [SerializeField]
    Sprite _imageHandleOff;

    [SerializeField]
    Sprite _imageHandleOn;

    [SerializeField]
    Color _normalColor;

    [SerializeField]
    Color _deactivateColor;

    [SerializeField]
    Color _pointerEnterColor;

    [SerializeField]
    Color _pointerDownColor;

    bool _isMouseIn;

    public override void OnToggle()
    {
        if (!IsActivated) return;

        if (IsOn)
        {
            _imageHandle.sprite = _imageHandleOn;
        }
        else
        {
            _imageHandle.sprite = _imageHandleOff;
        }
    }

    protected override void OnPointerClickInternal(BaseEventData data)
    {
        if (!IsActivated) return;
        SetToggle(!IsOn);
        base.OnPointerClickInternal(data);
    }

    public override void OnPointerEnter(BaseEventData data)
    {
        if (!IsActivated) return;
        _isMouseIn = true;
        _imageBG.DOColor(_pointerEnterColor, 0.2f);
    }
    public override void OnPointerUp(BaseEventData data)
    {
        if (!IsActivated) return;
        if (_isMouseIn)
        {
            _imageBG.DOColor(_pointerEnterColor, 0.2f);
        }
        else
        {
            _imageBG.DOColor(_normalColor, 0.2f);
        }
    }
    public override void OnPointerDown(BaseEventData data)
    {
        if (!IsActivated) return;
        _imageBG.DOColor(_pointerDownColor, 0.2f);
    }
    public override void OnPointerExit(BaseEventData data)
    {
        if (!IsActivated) return;
        {
            _isMouseIn = false;
            _imageBG.DOColor(_normalColor, 0.2f);
        }
    }
    public override void OnActivate()
    {
        _imageBG.DOColor(_normalColor, 0.2f);

    }
    public override void OnDeActivate()
    {
        _imageBG.DOColor(_deactivateColor, 0.2f);
    }
}
