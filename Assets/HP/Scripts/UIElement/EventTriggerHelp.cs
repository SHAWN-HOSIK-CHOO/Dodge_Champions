using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerHelp : MonoBehaviour
{
    // 코드로 UI 이벤트를 다루기 위해 작성

    [SerializeField]
    EventTrigger _eventTrigger;
    public void RemoveTriggerEvent(EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        if (GetType(eventType, out var entry))
        {
            entry.callback.RemoveListener(callback);
        }
    }
    public void AddTriggerEvent(EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        if (!GetType(eventType, out var entry))
        {
            entry = AddType(eventType);
        }
        entry.callback.AddListener(callback);
    }
    bool GetType(EventTriggerType eventType, out EventTrigger.Entry entry)
    {
        entry = null;
        foreach (var e in _eventTrigger.triggers)
        {
            if (e.eventID == eventType)
            {
                entry = e;
                return true;
            }
        }
        return false;
    }
    EventTrigger.Entry AddType(EventTriggerType eventType)
    {
        if (GetType(eventType, out var entry))
        {
            return entry;
        }
        EventTrigger.Entry newEntry = new EventTrigger.Entry();
        newEntry.eventID = eventType;
        _eventTrigger.triggers.Add(newEntry);
        return newEntry;
    }
    void RemoveType(EventTriggerType eventType)
    {
        if (GetType(eventType, out var entry))
        {
            _eventTrigger.triggers.Remove(entry);
        }
    }
}
