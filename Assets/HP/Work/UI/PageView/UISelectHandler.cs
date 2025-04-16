using System.Collections.Generic;
using UnityEngine;

public class UISelectHandler : MonoBehaviour
{
    List<UIImgToggle> _UISelectElements;

    UIImgToggle _currentSelected;
    public UIImgToggle CurrentSelected => _currentSelected;


    private void Awake()
    {
        _UISelectElements = new List<UIImgToggle>();
    }
    public void Remove(UIImgToggle element)
    {
        _UISelectElements.Remove(element);
        element.OnToggleAction -= OnToggle;
    }
    public void Add(UIImgToggle element)
    {
        _UISelectElements.Add(element);
        element.OnToggleAction += OnToggle;
    }
    public void Clear()
    {
        foreach (var element in _UISelectElements)
        {
            element.OnToggleAction -= OnToggle;
        }
        _UISelectElements.Clear();
    }
    private void OnToggle(GameObject obj)
    {
        var toggle = obj.GetComponent<UIImgToggle>();
        if (toggle.IsOn)
        {
            if (_currentSelected != null && _currentSelected.gameObject != obj)
            {
                _currentSelected.SetToggle(false);
                _currentSelected = null;
            }

            foreach (var element in _UISelectElements)
            {
                if (element.gameObject == obj)
                {
                    _currentSelected = element;
                    _currentSelected.SetToggle(true);
                }
            }
        }
    }
}
