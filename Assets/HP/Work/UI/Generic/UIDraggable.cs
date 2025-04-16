using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : UIElement
{
    [SerializeField]
    RectTransform _rectTransform;
    private Vector2 _dragOffset;
    protected override void Awake()
    {
        _useDrag = true;
        base.Awake();
    }
    public override void OnBeginDrag(BaseEventData data)
    {
        var pointerData = data as PointerEventData;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform.parent as RectTransform,
            pointerData.position,
            pointerData.pressEventCamera,
            out Vector2 localPoint))
        {
            _dragOffset = _rectTransform.anchoredPosition - localPoint;
        }

    }
    public override void OnDrag(BaseEventData data)
    {
        var pointerData = data as PointerEventData;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform.parent as RectTransform,
            pointerData.position,
            pointerData.pressEventCamera,
            out Vector2 localPoint))
        {
            _rectTransform.anchoredPosition = localPoint + _dragOffset;
        }


    }
    public override void OnEndDrag(BaseEventData data) { }
}
