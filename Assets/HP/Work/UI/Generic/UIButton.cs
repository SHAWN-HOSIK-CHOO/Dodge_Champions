using UnityEngine.EventSystems;

public class UIButton : UIElement
{
    protected override void Awake()
    {
        _usePointerUp = true;
        _usePointerDown = true;
        _usePointerClick = true;
        _usePointerEnter = true;
        _usePointerExit = true;
        base.Awake();
    }
    public override void OnPointerEnter(BaseEventData data)
    {

    }
    public override void OnPointerUp(BaseEventData data)
    {

    }
    public override void OnPointerDown(BaseEventData data)
    {

    }
    public override void OnPointerClick(BaseEventData data)
    {

    }
    public override void OnPointerExit(BaseEventData data)
    {

    }
}
