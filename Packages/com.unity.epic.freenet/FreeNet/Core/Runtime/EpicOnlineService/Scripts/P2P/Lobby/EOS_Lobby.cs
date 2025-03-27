using Epic.OnlineServices.Lobby;
using System.Collections.Generic;
using static EOSWrapper;

public class EOS_Lobby
{
    EOS_Core _eosCore;
    EOS_LobbyManager _lobbyManager;
    public bool _joined { get; private set; }
    public string _lobbyID { get; private set; }
    public EOSWrapper.ETC.PUID _localPUID { get; private set; }
    public EOSWrapper.ETC.PUID _lobbyOwner { get; private set; }
    public Dictionary<string, Epic.OnlineServices.Lobby.Attribute> _attribute;
    public Dictionary<EOSWrapper.ETC.PUID, EOS_LobbyMember> _members;
    public EOS_Lobby(EOS_Core eosCore, EOS_LobbyManager lobbyManager, string lobbyID, EOSWrapper.ETC.PUID localPUID)
    {
        _lobbyID = lobbyID;
        _localPUID = localPUID;
        _members = new Dictionary<EOSWrapper.ETC.PUID, EOS_LobbyMember>();
        _attribute = new Dictionary<string, Epic.OnlineServices.Lobby.Attribute>();
        _eosCore = eosCore;
        _lobbyManager = lobbyManager;
        _joined = true;
        if (ETC.ErrControl(EOSWrapper.LobbyControl.CopyLobbyDetailsByLobbyID(_eosCore._ILobby, _lobbyID, _localPUID._PUID, out var details)))
        {
            UpdateLobbyOwner(details);
            UpdateLobbyAttribute(details);
            UpdateMembers(details);
            details.Release();
        }
    }
    public void SetJoined(bool joined)
    {
        _joined = joined;
    }
    public bool GetLobbyInfo(out string info)
    {
        info = null;
        if (_attribute.TryGetValue("LOBBYINFO", out Epic.OnlineServices.Lobby.Attribute attr))
        {
            info = attr.Data.Value.Value.AsUtf8;
            return true;
        }
        return false;
    }
    public void UpdateMembers(LobbyDetails details)
    {
        uint memberCount = EOSWrapper.LobbyControl.GetCurrentMemberCount(details);
        Dictionary<EOSWrapper.ETC.PUID, EOS_LobbyMember> newMembers = new Dictionary<EOSWrapper.ETC.PUID, EOS_LobbyMember>();
        for (uint i = 0; i < memberCount; i++)
        {
            if (EOSWrapper.LobbyControl.GetMemberByIndex(details, i, out var puid))
            {
                var memberPUID = new EOSWrapper.ETC.PUID(puid);

                if (_members.TryGetValue(memberPUID, out var member))
                {
                    newMembers.Add(memberPUID, member);
                    _members.Remove(memberPUID);
                }
                else
                {
                    member = CreateLobbyMember(this, memberPUID);
                    newMembers.TryAdd(memberPUID, member);
                    _lobbyManager.InvokeMemberUpdate(member);
                }
                UpdateMembersAttribute(details, memberPUID);
            }
        }
        _members = newMembers;
    }
    public void UpdateLobbyOwner(LobbyDetails details)
    {
        if (EOSWrapper.LobbyControl.GetLobbyOwner(details, out var owner))
        {
            if (_lobbyOwner?.ToString() != owner.ToString())
            {
                _lobbyOwner = new EOSWrapper.ETC.PUID(owner);
            }
        }
    }
    public void UpdateLobbyAttribute(LobbyDetails details)
    {
        uint attrCount = EOSWrapper.LobbyControl.GetLobbyAttributeCount(details);
        _attribute.Clear();
        for (uint i = 0; i < attrCount; i++)
        {
            if (ETC.ErrControl(EOSWrapper.LobbyControl.GetLobbyAttributeByIndex(details, i, out var attr)))
            {

                if (!_attribute.TryGetValue(attr.Value.Data.Value.Key, out var _))
                {
                    _attribute.TryAdd(attr.Value.Data.Value.Key, attr.Value);
                    _lobbyManager.InvokeLobbyAttributeUpdate(attr.Value);
                }
                else
                {
                    if (EOSWrapper.ETC.Equal(_attribute[attr.Value.Data.Value.Key], attr.Value))
                    {
                        _attribute[attr.Value.Data.Value.Key] = attr.Value;
                        _lobbyManager.InvokeLobbyAttributeUpdate(attr.Value);
                    }
                }
            }
        }
    }
    public void UpdateMembersAttribute(LobbyDetails details, EOSWrapper.ETC.PUID memberPUID)
    {
        if (_members.TryGetValue(memberPUID, out var member))
        {
            uint attrCount = EOSWrapper.LobbyControl.GetMemberAttributeCount(details, memberPUID._PUID);
            member._attribute.Clear();
            for (uint i = 0; i < attrCount; i++)
            {
                if (EOSWrapper.LobbyControl.GetMemberAttributeByIndex(details, memberPUID._PUID, i, out var attr))
                {
                    if (!member._attribute.TryGetValue(attr.Value.Data.Value.Key, out var _))
                    {
                        member._attribute.TryAdd(attr.Value.Data.Value.Key, attr.Value);
                        _lobbyManager.InvokeMemberAttributeUpdate(member, attr.Value);
                    }
                    else
                    {
                        if (EOSWrapper.ETC.Equal(member._attribute[attr.Value.Data.Value.Key], attr.Value))
                        {
                            member._attribute[attr.Value.Data.Value.Key] = attr.Value;
                            _lobbyManager.InvokeMemberAttributeUpdate(member, attr.Value);
                        }
                    }
                }
            }
        }
    }
    public void UpdateMemberState(EOSWrapper.ETC.PUID memberPUID, LobbyMemberStatus state)
    {
        if (_members.TryGetValue(memberPUID, out var member))
        {
            if (member._state != state)
            {
                member.SetState(state);
                _lobbyManager.InvokeMemberStateUpdate(member);
            }
            if (member._state == LobbyMemberStatus.Left)
            {
                _members.Remove(member._localPUID);
            }
        }
        else if (state == LobbyMemberStatus.Joined)
        {
            member = CreateLobbyMember(this, memberPUID);
            _members.TryAdd(memberPUID, member);
            _lobbyManager.InvokeMemberStateUpdate(member);
        }
    }

    public virtual EOS_LobbyMember CreateLobbyMember(EOS_Lobby lobby, ETC.PUID puid)
    {
        return new EOS_LobbyMember(lobby, puid);
    }


    public void OnLobbyUpdateReceived(LobbyUpdateReceivedCallbackInfo info)
    {
        if (ETC.ErrControl(EOSWrapper.LobbyControl.CopyLobbyDetailsByLobbyID(_eosCore._ILobby, _lobbyID, _localPUID._PUID, out var details)))
        {
            UpdateLobbyAttribute(details);
            details.Release();
        }
    }
    public void OnLobbyMemberUpdateReceived(LobbyMemberUpdateReceivedCallbackInfo info)
    {
        if (ETC.ErrControl(EOSWrapper.LobbyControl.CopyLobbyDetailsByLobbyID(_eosCore._ILobby, _lobbyID, _localPUID._PUID, out var details)))
        {
            var puid = new EOSWrapper.ETC.PUID(info.TargetUserId);
            UpdateMembersAttribute(details, puid);
            details.Release();
        }
    }
    public void OnLobbyMemberStatusReceived(LobbyMemberStatusReceivedCallbackInfo info)
    {
        var puid = new EOSWrapper.ETC.PUID(info.TargetUserId);
        UpdateMemberState(puid, info.CurrentStatus);
    }
}