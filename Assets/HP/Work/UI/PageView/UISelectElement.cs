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
        if (s < 0.01f)
            s = 0.3f;
        float shiftedH = Mathf.Repeat(h + 0.1f, 1f);
        _SelectedColor = Color.HSVToRGB(shiftedH, s, v);
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
   
    public override void OnPointerExit(BaseEventData data)
    {
        if (!IsActivated) return;
        {
            _isMouseIn = false;
            if(_isSelected)
            {

                _image.DOColor(_SelectedColor, 0.2f);
            }
            else
            {
                _image.DOColor(_normalColor, 0.2f);
            }
        }
    }
    public override void OnActivate()
    {
        if (_isSelected)
        {
            _image.DOColor(_SelectedColor, 0.2f);
        }
        else
        {
            _image.DOColor(_normalColor, 0.2f);
        }

    }

    public override void OnPointerUp(BaseEventData data)
    {
        if (!IsActivated) return;
        if (_isMouseIn)
        {
            _image.DOColor(_pointerEnterColor, 0.2f);
        }
        else
        {
            if (_isSelected)
            {
                _image.DOColor(_SelectedColor, 0.2f);
            }
            else
            {
                _image.DOColor(_normalColor, 0.2f);
            }
        }
    }

}
