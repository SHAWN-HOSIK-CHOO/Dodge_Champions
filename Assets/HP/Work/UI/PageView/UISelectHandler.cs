using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectHandler : MonoBehaviour
{
    List<UISelectElement> _UISelectElements;

    UISelectElement _currentSelected;
    public UISelectElement CurrentSelected => _currentSelected;


    private void Awake()
    {
        _UISelectElements = new List<UISelectElement>();
    }
    public void Remove(UISelectElement element)
    {
        _UISelectElements.Remove(element);
    }
    public void Add(UISelectElement element)
    {
        _UISelectElements.Add(element);
        element.OnSelectAction += OnSelect;
    }
    public void Clear()
    {
        foreach (var element in _UISelectElements)
        {
            element.OnSelectAction -= OnSelect;
        }
        _UISelectElements.Clear();
    }
    private void OnSelect(BaseEventData data)
    {
        foreach(var element in _UISelectElements)
        {
            if (element == data.selectedObject)
            {
                if (_currentSelected != null && _currentSelected != element)
                {
                    _currentSelected.OnDeSelected();
                }
                _currentSelected = element;
                _currentSelected.OnSelected();
            }
        }
    }
}
