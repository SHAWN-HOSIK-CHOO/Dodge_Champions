using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class UIImgButton : UIButton
{
    Image _image;

    [SerializeField]
    Color _normalColor;

    [SerializeField]
    Color _deactivateColor;

    [SerializeField]
    Color _pointerEnterColor;

    [SerializeField]
    Color _pointerDownColor;

    bool _isMouseIn;

    protected override void Awake()
    {
        _image = GetComponent<Image>();
        base.Awake();
    }

    protected override void OnPointerClickInternal(BaseEventData data)
    {
        if (!IsActivated) return;
        OnPointerClickAction?.Invoke(data);
        OnPointerClick(data);
    }

    public override void OnPointerEnter(BaseEventData data)
    {
        if (!IsActivated) return;
        _isMouseIn = true;
        _image.DOColor(_pointerEnterColor, 0.2f);
    }
    public override void OnPointerUp(BaseEventData data)
    {
        if (!IsActivated) return;
        if (_isMouseIn)
        {
            _image.DOColor(_pointerEnterColor, 0.2f);
        }
        else
        {
            _image.DOColor(_normalColor, 0.2f);
        }
    }

    public override void OnPointerDown(BaseEventData data)
    {
        if (!IsActivated) return;
        _image.DOColor(_pointerDownColor, 0.2f);
    }

    public override void OnPointerExit(BaseEventData data)
    {
        if (!IsActivated) return;
        {
            _isMouseIn = false;
            _image.DOColor(_normalColor, 0.2f);
        }
    }

    public override void OnActivate()
    {
        _image.DOColor(_normalColor, 0.2f);

    }
    public override void OnDeActivate()
    {
        _image.DOColor(_deactivateColor, 0.2f);
    }

}
