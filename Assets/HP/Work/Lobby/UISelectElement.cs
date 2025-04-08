using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectElement : UIImgButton
{
    Color _SelectedColor;
    bool _isSelected;
    public bool IsSelected => _isSelected;
    public Action OnSelectedAction;
    public Action OnDeSelectedAction;

    protected override void GenerateColorsFrom(Color baseColor)
    {
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        float selectedS = Mathf.Clamp01(s + 0.3f);
        float selectedV = Mathf.Clamp01(v + 0.2f);
        _SelectedColor = Color.HSVToRGB(h, selectedS, selectedV);
        base.GenerateColorsFrom(baseColor);
    }
    protected virtual void OnSelectedInternal()
    {
        OnSelected();
        OnSelectedAction?.Invoke();
    }
    protected virtual void OnDeSelectedInternal()
    {
        OnDeSelected();
        OnDeSelectedAction?.Invoke();
    }
    public override void OnPointerClick(BaseEventData data)
    {
        OnSelectedInternal();
    }
    public void ChangeSelect(bool b)
    {
        if (b)
        { 
            OnSelectedInternal(); 
        }
        else
        {
            OnDeSelectedInternal();
        }

    }
    public virtual void OnSelected()
    {
        if (!IsActivated) return;
        _isSelected = true;
        GenerateColorsFrom(_SelectedColor);
        _image.DOColor(_SelectedColor, 0.2f);
    }
    public virtual void OnDeSelected()
    {
        if (!IsActivated) return;

        _isSelected = false;
        GenerateColorsFrom(_normalColor);
       _image.DOColor(_normalColor, 0.2f);
    }

}
