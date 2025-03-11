using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.UserInfo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyInfoElement : MonoBehaviour
{
    [SerializeField]
    TMP_Text _mode;
    [SerializeField]
    TMP_Text _owner;
    [SerializeField]
    TMP_Text _name;
    [SerializeField]
    TMP_Text _member;
    [SerializeField]
    Image _image;

    Color _originColor;
    FindJoinControl _findJoinControl;
    public EOS_LobbySearchResult _lobbySearch;
    TweenerCore<Color, Color, ColorOptions> _colorTween;
    private void OnDestroy()
    {
        var triggerHelper = GetComponent<EventTriggerHelper>();
        triggerHelper.RemoveTriggerEvent(EventTriggerType.PointerClick, onPointerClick);
        _lobbySearch.Release();
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
        _findJoinControl.SelecteMode(this);
        _colorTween.ChangeEndValue(Color.cyan, 0.5f).Restart();
    }

    
    public void Init(FindJoinControl control, EOS_LobbySearchResult lobby)
    {
        _findJoinControl = control;
        _lobbySearch = lobby;
        if(SimpleLobbyAttributeExtenstion.GetLobbyMode(_lobbySearch._attribute, out var mode))
        {
            _mode.text = mode;
        }
        if (SimpleLobbyAttributeExtenstion.GetLobbyName(_lobbySearch._attribute, out var name))
        {
            _name.text = name;
        }
        if (SimpleLobbyAttributeExtenstion.GetOwner(_lobbySearch._attribute, out var owner))
        {
            _owner.text = owner;
        }
        _member.text = $"{_lobbySearch._info.AvailableSlots} / {_lobbySearch._info.MaxMembers}";
    }

    void onPointerClick(BaseEventData data)
    {
        if (_findJoinControl.curSelectedLobby!= null)
        {
            if (_findJoinControl.curSelectedLobby._lobbySearch == _lobbySearch)
            {
                _findJoinControl.InvokeJoin(this);
                return;
            }
        }
        Select();
    }
}
