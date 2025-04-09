using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerHelper
{
    public static void RemoveTriggerEvent(EventTrigger eventTrigger,EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        if (GetType(eventTrigger,eventType, out var entry))
        {
            entry.callback.RemoveListener(callback);
        }
    }
    public static void AddTriggerEvent(EventTrigger eventTrigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        if (!GetType(eventTrigger,eventType, out var entry))
        {
            entry = AddType(eventTrigger,eventType);
        }
        entry.callback.AddListener(callback);
    }
    static bool GetType(EventTrigger eventTrigger, EventTriggerType eventType, out EventTrigger.Entry entry)
    {
        entry = null;
        foreach (var e in eventTrigger.triggers)
        {
            if (e.eventID == eventType)
            {
                entry = e;
                return true;
            }
        }
        return false;
    }
    static EventTrigger.Entry AddType(EventTrigger eventTrigger, EventTriggerType eventType)
    {
        if (GetType(eventTrigger,eventType, out var entry))
        {
            return entry;
        }
        EventTrigger.Entry newEntry = new EventTrigger.Entry();
        newEntry.eventID = eventType;
        eventTrigger.triggers.Add(newEntry);
        return newEntry;
    }
    public static void RemoveType(EventTrigger eventTrigger, EventTriggerType eventType)
    {
        if (GetType(eventTrigger,eventType, out var entry))
        {
            eventTrigger.triggers.Remove(entry);
        }
    }
}
