using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModeElement : MonoBehaviour
{
    [SerializeField]
    Image _image;
    [SerializeField]
    public TMP_Text _text;
    Color _originColor;
    CreateControl _createControl;

    TweenerCore<Color, Color, ColorOptions> _colorTween;
    private void OnDestroy()
    {
        var triggerHelper = GetComponent<EventTriggerHelper>();
        triggerHelper.RemoveTriggerEvent(EventTriggerType.PointerClick, onPointerClick);
    }
    void Awake()
    {
        var triggerHelper = GetComponent<EventTriggerHelper>();
        triggerHelper.AddTriggerEvent(EventTriggerType.PointerClick, onPointerClick);
        _originColor = _image.color;
        _colorTween = _image.DOColor(_originColor, 0f).SetAutoKill(false);
    }
    public void UnSelect()
    {
        _colorTween.ChangeEndValue(_originColor, 0.5f).Restart();
    }
    public void Select()
    {
        _createControl.SelecteMode(this);
        _colorTween.ChangeEndValue(Color.cyan, 0.5f).Restart();
    }
    public void Init(CreateControl control,string mode)
    {
        _createControl = control;
        _text.text = mode;
    }

    void onPointerClick(BaseEventData data)
    {
        Select();
    }
}
