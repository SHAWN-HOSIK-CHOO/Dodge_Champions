using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class UIImgButton : UIButton
{
    protected Image _image;

    [SerializeField]
    protected Color _normalColor;

    protected Color _deactivateColor;

    protected Color _pointerEnterColor;

    protected Color _pointerDownColor;

    protected bool _isMouseIn;

    protected override void Awake()
    {
        _image = GetComponent<Image>();
        GenerateColorsFrom(_normalColor);
        base.Awake();
    }
    protected virtual void GenerateColorsFrom(Color baseColor)
    {
        _deactivateColor = Color.red;
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        float enterS = Mathf.Clamp01(s + 0.2f);
        float downS = Mathf.Clamp01(s + 0.4f);
        _pointerEnterColor = Color.HSVToRGB(h, enterS, v);
        _pointerDownColor = Color.HSVToRGB(h, downS, v);
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
