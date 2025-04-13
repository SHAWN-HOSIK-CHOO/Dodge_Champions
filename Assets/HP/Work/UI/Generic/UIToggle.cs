using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIToggle : UIImgButton
{
    [SerializeField]
    protected bool _isOn;
    public bool IsOn => _isOn;
    public Action<GameObject> OnToggleAction;
    protected override void Awake()
    {
        _usePointerUp = true;
        _usePointerDown = true;
        _usePointerClick = true;
        _usePointerEnter = true;
        _usePointerExit = true;
        base.Awake();
        OnToggleInternal(IsOn);
    }
    protected virtual void OnToggle(bool b)
    {
        if (!IsActivated) return;
        _isOn = b;
    }
    protected override void OnPointerClickInternal(BaseEventData data)
    {
        if (!IsActivated) return;
        OnToggleInternal(!IsOn);
        base.OnPointerClickInternal(data);
    }
    void OnToggleInternal(bool b)
    {
        if (!IsActivated) return;
        if (b == IsOn) return;
        OnToggle(b);
        OnToggleAction?.Invoke(this.gameObject);
    }
    public void SetToggle(bool b)
    {
        OnToggleInternal(b);
    }
}
