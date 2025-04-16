using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
        GenerateColors();
        base.Awake();
    }
    protected virtual void GenerateColors()
    {
        _deactivateColor = Color.red;
        Color.RGBToHSV(_normalColor, out float h, out float s, out float v);
        float enterV = Mathf.Clamp01(v - 0.1f); // »ìÂ¦ ¾îµÓ°Ô
        float downV = Mathf.Clamp01(v - 0.2f);  // ´õ ¾îµÓ°Ô
        _pointerEnterColor = Color.HSVToRGB(h, s, enterV);
        _pointerDownColor = Color.HSVToRGB(h, s, downV);
    }
    protected override void OnPointerClickInternal(BaseEventData data)
    {
        if (!IsActivated) return;
        OnPointerClick(data);
        OnPointerClickAction?.Invoke(data);
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
