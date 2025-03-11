using Epic.OnlineServices.Lobby;
using Epic.OnlineServices;
using System.Collections.Generic;
using System;
using UnityEngine;
using static EOSWrapper;

public abstract class EOS_LobbyManager : MonoBehaviour 
{
    protected EOS_Core _eosCore;
    protected EOS_LocalUser _localUser;
    protected Dictionary<string, EOS_Lobby> _lobbies;
    public virtual void OnRelease()
    {
        RemoveLobbyCallback();
    }
    public virtual void Init(FreeNet freenet)
    {
        _eosCore = freenet._eosCore;
        _localUser = freenet._localUser;
        _lobbies = new Dictionary<string, EOS_Lobby>();
        AddLobbyCallback();
    }

    #region EOScallbacks
    ulong _onLobbyUpdateReceivedHandle;
    ulong _onLobbyMemberUpdateReceivedHandle;
    ulong _onLobbyMemberStatusReceivedHandle;
    ulong _onLeaveLobbyRequestedHandle;
    ulong _onLobbyInviteReceivedHandle;
    ulong _onLobbyInviteAcceptedHandle;
    ulong _onLobbyInviteRejectedHandle;
    ulong _onJoinLobbyAcceptedHandle;
    #endregion

    public event Action<Epic.OnlineServices.Lobby.Attribute> _onLobbyAttributeUpdate;
    public event Action<EOS_LobbyMember, Epic.OnlineServices.Lobby.Attribute> _onMemberAttributeUpdate;
    public event Action<EOS_LobbyMember> _onMemberUpdate;
    public event Action<EOS_LobbyMember> _onMemberStateUpdate;
    public event Action<LobbyInviteRejectedCallbackInfo> _onInviteRejected;
    public event Action<LobbyInviteReceivedCallbackInfo> _onInviteReceived;

    public void CreateLobby(CreateLobbyOptions options, Epic.OnlineServices.Lobby.AttributeData[] searchParams, Action<Result, EOS_Lobby> onComplete = null)
    {
        EOSWrapper.LobbyControl.CreateLobby(_eosCore._ILobby, options, (ref CreateLobbyCallbackInfo info) =>
        {
            if (EOSWrapper.ETC.ErrControl<EOS_Lobby>(info.ResultCode, onComplete))
            {
                if (ETC.ErrControl<EOS_Lobby>(EOSWrapper.LobbyControl.GetLobbyModification(_eosCore._ILobby, info.LobbyId, _localUser._localPUID._PUID, out var modification), onComplete))
                {
                    foreach (var item in searchParams)
                    {
                        if (ETC.ErrControl<EOS_Lobby>(EOSWrapper.LobbyControl.SetModificationAddLobbyAttribute(modification, new Epic.OnlineServices.Lobby.Attribute()
                        {
                            Visibility = LobbyAttributeVisibility.Public,
                            Data = item
                        }), onComplete))
                        {
                            continue;
                        }
                    }
                    EOSWrapper.LobbyControl.UpdateLobby(_eosCore._ILobby, modification, (ref UpdateLobbyCallbackInfo info) =>
                    {
                        if (ETC.ErrControl<EOS_Lobby>(info.ResultCode, onComplete))
                        {
                            var lobby = CreateJoinedLobby(_eosCore, this, info.LobbyId);
                            modification.Release();
                            onComplete?.Invoke(info.ResultCode, lobby);
                        }
                    });
                }
            }
        });
    }
    public void FindLobby(uint findNum, List<Epic.OnlineServices.Lobby.AttributeData> searchParams, Action<Result, List<EOS_LobbySearchResult>> onComplete = null)
    {
        if (EOSWrapper.ETC.ErrControl(EOSWrapper.LobbyControl.GetLobbySearch(_eosCore._ILobby, findNum, out var search), onComplete))
        {
            foreach (var item in searchParams)
            {
                if (!ETC.ErrControl(EOSWrapper.LobbyControl.SetSearchParamAttribute(search, item, ComparisonOp.Equal), onComplete))
                {
                    return;
                }
            }
            EOSWrapper.LobbyControl.SearchLobby(search, _localUser._localPUID._PUID, (ref LobbySearchFindCallbackInfo info) =>
            {
                if (ETC.ErrControl(info.ResultCode, onComplete))
                {
                    uint count = EOSWrapper.LobbyControl.GetSearchResultCount(search);
                    var findlobbies = new List<EOS_LobbySearchResult>();
                    List<Epic.OnlineServices.Lobby.AttributeData> attrList = new List<AttributeData>();
                    for (uint i = 0; i < count; i++)
                    {
                        if (ETC.ErrControl(EOSWrapper.LobbyControl.CopySearchResultByIndex(search, i, out var details), onComplete))
                        {
                            var lobby = new EOS_LobbySearchResult(this, details);
                            findlobbies.Add(lobby);
                        }
                    }
                    onComplete?.Invoke(Result.Success, findlobbies);
                }
                search.Release();
            });
        }
    }
    public void JoinLobbyByDetails(LobbyDetails details, Action<Result, EOS_Lobby> onComplete = null)
    {
        EOSWrapper.LobbyControl.JoinLobbyByDetails(_eosCore._ILobby, true, details, _localUser._localPUID._PUID, (ref JoinLobbyCallbackInfo info) =>
        {
            if (EOSWrapper.ETC.ErrControl(info.ResultCode, onComplete))
            {
                var lobby = CreateJoinedLobby(_eosCore, this, info.LobbyId);
                onComplete?.Invoke(info.ResultCode, lobby);
            }
        });
    }
    public void JoinLobbyById(string lobbyID, Action<Result, EOS_Lobby> onComplete = null)
    {
        EOSWrapper.LobbyControl.JoinLobbyById(_eosCore._ILobby, true, lobbyID, _localUser._localPUID._PUID, (ref JoinLobbyByIdCallbackInfo info) =>
        {
            if (EOSWrapper.ETC.ErrControl(info.ResultCode, onComplete))
            {
                var lobby = CreateJoinedLobby(_eosCore, this, info.LobbyId);
                onComplete?.Invoke(info.ResultCode, lobby);
            }
        });
    }
    public void LeaveLobby(EOS_Lobby lobby, Action<Result, EOS_Lobby> onComplete = null)
    {
        EOSWrapper.LobbyControl.LeaveLobby(_eosCore._ILobby, lobby._lobbyID, lobby._localPUID._PUID, (ref LeaveLobbyCallbackInfo info) =>
        {
            if (ETC.ErrControl(info.ResultCode, onComplete))
            {
                lobby.SetJoined(false);

                Debug.LogError("A lobby already exists. Duplicate creation detected.");
                _lobbies.Remove(lobby._lobbyID);
                onComplete?.Invoke(info.ResultCode, lobby);
            }
        });
    }
    public void RemoveLobbyCallback()
    {
        _eosCore._ILobby.RemoveNotifyLobbyUpdateReceived(_onLobbyUpdateReceivedHandle);
        _eosCore._ILobby.RemoveNotifyLobbyMemberUpdateReceived(_onLobbyMemberUpdateReceivedHandle);
        _eosCore._ILobby.RemoveNotifyLobbyMemberStatusReceived(_onLobbyMemberStatusReceivedHandle);
        _eosCore._ILobby.RemoveNotifyLeaveLobbyRequested(_onLeaveLobbyRequestedHandle);
        _eosCore._ILobby.RemoveNotifyLobbyInviteReceived(_onLobbyInviteReceivedHandle);
        _eosCore._ILobby.RemoveNotifyLobbyInviteAccepted(_onLobbyInviteAcceptedHandle);
        _eosCore._ILobby.RemoveNotifyLobbyInviteRejected(_onLobbyInviteRejectedHandle);
        _eosCore._ILobby.RemoveNotifyJoinLobbyAccepted(_onJoinLobbyAcceptedHandle);
    }
    public void AddLobbyCallback()
    {
        _onLobbyUpdateReceivedHandle = EOSWrapper.LobbyControl.AddCBNotifyLobbyUpdateReceived(_eosCore._ILobby, OnLobbyUpdateReceived);
        _onLobbyMemberUpdateReceivedHandle = EOSWrapper.LobbyControl.AddCBNotifyLobbyMemberUpdateReceived(_eosCore._ILobby, OnLobbyMemberUpdateReceived);
        _onLobbyMemberStatusReceivedHandle = EOSWrapper.LobbyControl.AddCBNotifyLobbyMemberStatusReceived(_eosCore._ILobby, OnLobbyMemberStatusReceived);
        _onLeaveLobbyRequestedHandle = EOSWrapper.LobbyControl.AddCBNotifyLeaveLobbyRequested(_eosCore._ILobby, OnLeaveLobbyRequested);
        _onLobbyInviteReceivedHandle = EOSWrapper.LobbyControl.AddCBNotifyLobbyInviteReceived(_eosCore._ILobby, OnLobbyInviteReceived);
        _onLobbyInviteAcceptedHandle = EOSWrapper.LobbyControl.AddCBNotifyLobbyInviteAccepted(_eosCore._ILobby, OnLobbyInviteAccepted);
        _onLobbyInviteRejectedHandle = EOSWrapper.LobbyControl.AddCBNotifyLobbyInviteRejected(_eosCore._ILobby, OnLobbyInviteRejected);
        _onJoinLobbyAcceptedHandle = EOSWrapper.LobbyControl.AddCBNotifyJoinLobbyAccepted(_eosCore._ILobby, OnJoinLobbyAccepted);
    }


    public virtual EOS_Lobby CreateLobby(EOS_Core eosCore, EOS_LobbyManager lobbyManager, string lobbyID)
    {
        return new EOS_Lobby(eosCore, lobbyManager, lobbyID, _localUser._localPUID);
    }

    private EOS_Lobby CreateJoinedLobby(EOS_Core eosCore, EOS_LobbyManager lobbyManager, string lobbyID)
    {
        var lobby = CreateLobby(eosCore, lobbyManager, lobbyID);
        if(!_lobbies.TryAdd(lobbyID, lobby))
        {
            Debug.LogError("A lobby already exists. Duplicate creation detected.");
        }
        return lobby;
    }
    public void InvokeLobbyAttributeUpdate(Epic.OnlineServices.Lobby.Attribute attribute)
    {
        _onLobbyAttributeUpdate?.Invoke(attribute);
    }
    public void InvokeMemberAttributeUpdate(EOS_LobbyMember member, Epic.OnlineServices.Lobby.Attribute attribute)
    {
        _onMemberAttributeUpdate?.Invoke(member, attribute);
    }
    public void InvokeMemberUpdate(EOS_LobbyMember member)
    {
        _onMemberUpdate?.Invoke(member);
    }
    public void InvokeMemberStateUpdate(EOS_LobbyMember member)
    {
        _onMemberStateUpdate?.Invoke(member);
    }
    public void InvokeInviteRejected(LobbyInviteRejectedCallbackInfo info)
    {
        _onInviteRejected?.Invoke(info);
    }
    public void InvokeInviteReceived(LobbyInviteReceivedCallbackInfo info)
    {
        _onInviteReceived?.Invoke(info);
    }
    public bool GetLobby(string lobbyID, out EOS_Lobby lobby)
    {
        return _lobbies.TryGetValue(lobbyID, out lobby);    
    }
    void OnLobbyUpdateReceived(ref LobbyUpdateReceivedCallbackInfo info)
    {
        if(GetLobby(info.LobbyId,out var lobby))
        {
            lobby.OnLobbyUpdateReceived(info);
        }
    }
    void OnLobbyMemberUpdateReceived(ref LobbyMemberUpdateReceivedCallbackInfo info)
    {
        if (GetLobby(info.LobbyId, out var lobby))
        {
            lobby.OnLobbyMemberUpdateReceived(info);
        }
    }
    void OnLobbyMemberStatusReceived(ref LobbyMemberStatusReceivedCallbackInfo info)
    {
        if (GetLobby(info.LobbyId, out var lobby))
        {
            lobby.OnLobbyMemberStatusReceived(info);
            if (info.CurrentStatus == LobbyMemberStatus.Left)
            {
                if (info.TargetUserId.ToString() == lobby._lobbyOwner.ToString())
                {
                    //we leaved Lobby;
                }
            }
        }
    }
    void OnLeaveLobbyRequested(ref LeaveLobbyRequestedCallbackInfo info)
    {
        if (GetLobby(info.LobbyId, out var lobby))
        {
            LeaveLobby(lobby);
        }
    }
    void OnLobbyInviteAccepted(ref LobbyInviteAcceptedCallbackInfo info)
    {
        if (EOSWrapper.LobbyControl.CopyLobbyDetailsByInviteID(_eosCore._ILobby, info.InviteId, out var details))
        {
            JoinLobbyByDetails(details,(Result result, EOS_Lobby lobby) =>
            {
                details.Release();
            });
        }
    }
    void OnLobbyInviteRejected(ref LobbyInviteRejectedCallbackInfo info)
    {
        _onInviteRejected?.Invoke(info);
    }
    void OnLobbyInviteReceived(ref LobbyInviteReceivedCallbackInfo info)
    {
        _onInviteReceived?.Invoke(info);
    }
    void OnJoinLobbyAccepted(ref JoinLobbyAcceptedCallbackInfo info)
    {
        if (EOSWrapper.LobbyControl.CopyLobbyDetailsByUI(_eosCore._ILobby, info.UiEventId, out var details))
        {
            JoinLobbyByDetails(details, (Result result, EOS_Lobby lobby) =>
            {
                details.Release();
            });
        }
    }
}
