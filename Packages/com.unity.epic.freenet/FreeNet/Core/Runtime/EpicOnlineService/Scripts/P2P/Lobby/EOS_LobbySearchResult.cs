using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using System;
using System.Collections.Generic;
using UnityEngine;
using static EOSWrapper;

public class EOS_LobbySearchResult
{
    public LobbyDetailsInfo _info;
    EOS_LobbyManager _lobbyManager;
    public Dictionary<string, Epic.OnlineServices.Lobby.Attribute> _attribute;
    LobbyDetails _details;
    public EOS_LobbySearchResult(EOS_LobbyManager lobbyManager, LobbyDetails details)
    {
        _lobbyManager = lobbyManager;
        _details = details;
        _attribute = new Dictionary<string, Epic.OnlineServices.Lobby.Attribute>();

        uint attrCount = EOSWrapper.LobbyControl.GetLobbyAttributeCount(details);
        for (uint k = 0; k < attrCount; k++)
        {
            if (ETC.ErrControl(EOSWrapper.LobbyControl.GetLobbyAttributeByIndex(details, k, out var attr)))
            {
                _attribute.Add(attr.Value.Data.Value.Key, attr.Value);
            }
        }
        if (ETC.ErrControl(EOSWrapper.LobbyControl.GetLobbyDetailsInfo(_details, out var info)))
        {
            _info = info.Value;
        }
    }
    public void Release()
    {
        if(_details!=null)
        {
            _details.Release();
            _details = null;
        }
    }
    public void JoinLobby(Action<Result, EOS_Lobby> onComplete = null)
    {
        _lobbyManager.JoinLobbyByDetails(_details, (Result result, EOS_Lobby lobby) =>
        {
            _details.Release();
            _details = null;
        });
    }
}
