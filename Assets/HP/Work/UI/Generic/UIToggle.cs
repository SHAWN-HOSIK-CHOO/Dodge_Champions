using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIToggle : UIElement
{
    [SerializeField]
    bool _isOn;
    public bool IsOn => _isOn;
    public Action OnToggleAction;


    protected override void Awake()
    {
        _usePointerUp = true;
        _usePointerDown = true;
        _usePointerClick = true;
        _usePointerEnter = true;
        _usePointerExit = true;
        base.Awake();
        SetToggle(IsOn);
    }
    public void SetToggle(bool b)
    {
        if (!IsActivated) return;
        _isOn = b;
        OnToggle();
        OnToggleAction?.Invoke();
    }
    public virtual void OnToggle()
    {

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
