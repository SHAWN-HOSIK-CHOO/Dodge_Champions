using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIElement : MonoBehaviour
{
    [SerializeField]
    private bool _isActivated;

    public bool IsActivated => _isActivated;

    protected bool _useDrag;
    protected bool _useSelect;
    protected bool _usePointerUp;
    protected bool _usePointerClick;
    protected bool _usePointerEnter;
    protected bool _usePointerExit;
    protected bool _usePointerDown;

    public Action<GameObject> OnActivateAction;
    public Action<GameObject> OnDeActivateAction;
    public Action<BaseEventData> OnBeginDragAction;
    public Action<BaseEventData> OnDragAction;
    public Action<BaseEventData> OnEndDragAction;
    public Action<BaseEventData> OnPointerUpAction;
    public Action<BaseEventData> OnPointerClickAction;
    public Action<BaseEventData> OnPointerEnterAction;
    public Action<BaseEventData> OnPointerExitAction;
    public Action<BaseEventData> OnPointerDownAction;

    protected InputActionAsset _inputActionAsset;
    const string _inputActionAssetName = "UI";


    protected virtual void Awake()
    {
        var eventTrigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        if (_useDrag)
        {
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.BeginDrag, OnBeginDragInternal);
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.Drag, OnDragInternal);
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.EndDrag, OnEndDragInternal);
        }
        if (_usePointerUp)
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerUp, OnPointerUpInternal);
        if (_usePointerClick)
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerClick, OnPointerClickInternal);
        if (_usePointerEnter)
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerEnter, OnPointerEnterInternal);
        if (_usePointerExit)
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerExit, OnPointerExitInternal);
        if (_usePointerDown)
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerDown, OnPointerDownInternal);
        if (_isActivated)
            Activate();
        else
            DeActivate();
    }

    protected virtual void OnEnable()
    {
        Activate();
    }

    protected virtual void OnDisable()
    {
        DeActivate();
    }

    public void Activate()
    {
        _isActivated = true;
        OnActivate();
        OnActivateAction?.Invoke(this.gameObject);
    }

    public void DeActivate()
    {
        _isActivated = false;
        OnDeActivate();
        OnDeActivateAction?.Invoke(this.gameObject);
    }

    protected virtual void OnBeginDragInternal(BaseEventData data)
    {
        OnBeginDrag(data);
        OnBeginDragAction?.Invoke(data);
    }
    protected virtual void OnDragInternal(BaseEventData data)
    {
        OnDrag(data);
        OnDragAction?.Invoke(data);
    }
    protected virtual void OnEndDragInternal(BaseEventData data)
    {
        OnEndDrag(data);
        OnEndDragAction?.Invoke(data);
    }

    protected virtual void OnPointerUpInternal(BaseEventData data)
    {
        OnPointerUp(data);
        OnPointerUpAction?.Invoke(data);
    }

    protected virtual void OnPointerClickInternal(BaseEventData data)
    {
        OnPointerClick(data);
        OnPointerClickAction?.Invoke(data);
    }

    protected virtual void OnPointerEnterInternal(BaseEventData data)
    {
        OnPointerEnter(data);
        OnPointerEnterAction?.Invoke(data);
    }

    protected virtual void OnPointerExitInternal(BaseEventData data)
    {
        OnPointerExit(data);
        OnPointerExitAction?.Invoke(data);
    }

    protected virtual void OnPointerDownInternal(BaseEventData data)
    {
        OnPointerDown(data);
        OnPointerDownAction?.Invoke(data);
    }

    public virtual void OnActivate() { }
    public virtual void OnDeActivate() { }
    public virtual void OnBeginDrag(BaseEventData data) { }
    public virtual void OnDrag(BaseEventData data) { }
    public virtual void OnEndDrag(BaseEventData data) { }
    public virtual void OnPointerUp(BaseEventData data) { }
    public virtual void OnPointerClick(BaseEventData data) { }
    public virtual void OnPointerEnter(BaseEventData data) { }
    public virtual void OnPointerExit(BaseEventData data) { }
    public virtual void OnPointerDown(BaseEventData data) { }
}
