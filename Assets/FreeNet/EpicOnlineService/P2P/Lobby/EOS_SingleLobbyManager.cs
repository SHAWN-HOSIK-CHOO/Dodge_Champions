using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using System;
using System.Collections.Generic;
using UnityEngine;
using static EOSWrapper;

/*
 * NGO는 하나의 네트워크 매니저를 두어야 하기 때문에  
 * 단일 로비를 유지해야하며 또한 호스트 마이그레이션 지원 안됨으로 아래와 같은 구조로 게임 진행
 * [로비 검색 -> Active 로비 -> Session -> Active 로비]
 */

public class EOS_SingleLobbyManager : SingletonMonoBehaviour<EOS_SingleLobbyManager>
{
    public enum LobbyType
    {
        Public,
        Protected,
    }
    EOS_Lobby _currentLobby;
    EOS_Core _eosCore;
    EOS_LocalUser _localUser;
    private void Awake()
    {
        SingletonSpawn(this);
    }
    public class FoundLobby
    {
        public LobbyDetailsInfo _info;
        EOS_SingleLobbyManager _lobbyManager;
        public Dictionary<string, Epic.OnlineServices.Lobby.Attribute> _attribute;
        LobbyDetails _details;

        public FoundLobby(EOS_SingleLobbyManager lobbyManager, LobbyDetails details)
        {
            _lobbyManager = lobbyManager;
            _details = details;
             _attribute = new Dictionary<string, Epic.OnlineServices.Lobby.Attribute>();

            uint attrCount = EOSWrapper.LobbyControl.GetLobbyAttributeCount(details);
            for (uint k = 0; k < attrCount; k++)
            {
                if (ETC.ErrControl(LobbyControl.GetLobbyAttributeByIndex(details, k, out var attr)))
                {
                    _attribute.Add(attr.Value.Data.Value.Key, attr.Value);
                }
            }
            if (ETC.ErrControl(LobbyControl.GetLobbyDetailsInfo(_details, out var info)))
            {
                _info = info.Value;
            }
        }
        
        public void Release()
        {
            if(_details == null)
            {
                _details.Release();
            }
        }
        public void JoinLobby()
        {
            if(_details!= null)
            {
               _lobbyManager.JoinLobby(_details);
            }
            _details = null;
        }
    }
    public void Release()
    {
        RemoveLobbyCallback();
    }
    public void Init(EOS_Core eosCore, EOS_LocalUser localUser)
    {
        _eosCore = eosCore;
        _localUser = localUser;
        _currentLobby = null;
        AddLobbyCallback();
        SingletonInitialize();
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
    #region callbacks
    public event Action<Epic.OnlineServices.Lobby.Attribute> _onLobbyAttributeUpdate;
    public event Action<EOS_LobbyMember, Epic.OnlineServices.Lobby.Attribute> _onMemberAttributeUpdate;
    public event Action<EOS_LobbyMember> _onMemberUpdate;
    public event Action<EOS_LobbyMember> _onMemberStateUpdate;
    public event Action<LobbyInviteRejectedCallbackInfo> _onInviteRejected;
    public event Action<LobbyInviteReceivedCallbackInfo> _onInviteReceived;
    public event Action<Result,EOS_Lobby> _onJoinLobby;
    public event Action<Result,EOS_Lobby> _onLeaveLobby;
    #endregion
    public void CreateLobby(uint maxLobbyMember, LobbyType type, string lobbyInfo)
    {
        Epic.OnlineServices.Lobby.AttributeData[] searchParams = new Epic.OnlineServices.Lobby.AttributeData[3]
{
            new AttributeData{ Key = "LOBBYTYPE", Value = type.ToString()},
            new AttributeData { Key = "LOBBYCODE", Value = GenerateLobbyCode()},
            new AttributeData { Key = "LOBBYINFO", Value = lobbyInfo}
        };
        var options = new CreateLobbyOptions()
        {
            LocalUserId = _localUser.GetLocalPUID(),
            MaxLobbyMembers = maxLobbyMember,
            PermissionLevel = LobbyPermissionLevel.Publicadvertised,
            BucketId = type.ToString(),
            PresenceEnabled = true,
            AllowInvites = true,
            DisableHostMigration = true,
            EnableRTCRoom = false, // 음성 채팅 
            EnableJoinById = true,
        };
        LobbyControl.CreateLobby(_eosCore._ILobby, options, (ref CreateLobbyCallbackInfo info) =>
        {
            if(EOSWrapper.ETC.ErrControl<EOS_Lobby>(info.ResultCode,_onJoinLobby))
            {
                if (ETC.ErrControl<EOS_Lobby>(LobbyControl.GetLobbyModification(_eosCore._ILobby, info.LobbyId, _localUser.GetLocalPUID(), out var modification),_onJoinLobby))
                {
                    foreach (var item in searchParams)
                    {
                        if(ETC.ErrControl<EOS_Lobby>(LobbyControl.SetModificationAddLobbyAttribute(modification, new Epic.OnlineServices.Lobby.Attribute()
                        {
                            Visibility = LobbyAttributeVisibility.Public,
                            Data = item
                        }), _onJoinLobby))
                        {
                            continue;
                        }
                    }
                    LobbyControl.UpdateLobby(_eosCore._ILobby, modification, (ref UpdateLobbyCallbackInfo info) =>
                    {
                        if(ETC.ErrControl<EOS_Lobby>(info.ResultCode,_onJoinLobby))
                        {
                            var lobby = CreateJoinedLobby(info.LobbyId);
                            modification.Release();
                        }
                    });
                }
            }
        });
    }
    public void FindLobbyByCode(uint findNum, string lobbyCode, Action<Result, List<FoundLobby>> onComplete = null)
    {
        List<Epic.OnlineServices.Lobby.AttributeData> searchParams = new List<AttributeData>();
        searchParams.Add(new AttributeData { Key = "LOBBYCODE", Value = lobbyCode });
        FindLobby(findNum, searchParams, onComplete);
    }
    public void FindPublicLobby(uint findNum, string lobbyCode = null, string lobbyInfo = null, Action<Result, List<FoundLobby>> onComplete = null)
    {
        List<Epic.OnlineServices.Lobby.AttributeData> searchParams = new List<AttributeData>();
        searchParams.Add(new AttributeData { Key = "LOBBYTYPE", Value = LobbyType.Public.ToString() });
        if (lobbyCode != null)
        {
            searchParams.Add(new AttributeData { Key = "LOBBYCODE", Value = lobbyCode });
        }

        FindLobby(findNum, searchParams,onComplete);
    }
    void FindLobby(uint findNum, List<Epic.OnlineServices.Lobby.AttributeData> searchParams, Action<Result, List<FoundLobby>> onComplete= null)
    {
        if (EOSWrapper.ETC.ErrControl(LobbyControl.GetLobbySearch(_eosCore._ILobby, findNum, out var search), onComplete))
        {
            foreach (var item in searchParams)
            {
                if (!ETC.ErrControl(LobbyControl.SetSearchParamAttribute(search, item, ComparisonOp.Equal), onComplete))
                {
                    return;
                }
            }
            LobbyControl.SearchLobby(search, _localUser.GetLocalPUID(), (ref LobbySearchFindCallbackInfo info) =>
            {
                if (ETC.ErrControl(info.ResultCode, onComplete))
                {
                    uint count = LobbyControl.GetSearchResultCount(search);
                    var findlobbies = new List<FoundLobby>();
                    List<Epic.OnlineServices.Lobby.AttributeData> attrList = new List<AttributeData>();
                    for (uint i = 0; i < count; i++)
                    {
                        if (ETC.ErrControl(LobbyControl.CopySearchResultByIndex(search, i, out var details),onComplete))
                        {
                            var lobby = new FoundLobby(this, details);
                            findlobbies.Add(lobby);
                        }
                    }
                    onComplete?.Invoke(Result.Success,findlobbies);
                }
                search.Release();
            });
        }
    }
    public void JoinLobby(LobbyDetails details)
    {
        if (!IsCurrentJoined(details))
        {
            LeaveLobby();
            EOSWrapper.LobbyControl.JoinLobbyByDetails(_eosCore._ILobby, true, details, _localUser.GetLocalPUID(), (ref JoinLobbyCallbackInfo info) =>
            {
                if (info.ResultCode == Epic.OnlineServices.Result.Success)
                {
                    CreateJoinedLobby(info.LobbyId);
                }
                details.Release();
            });
        }
    }
    public void LeaveLobby()
    {
        if (_currentLobby == null)
        {
            _onLeaveLobby?.Invoke(Result.Success,null);
        }
        else
        {
            EOS_Lobby lobby = _currentLobby;
            _currentLobby = null;
            EOSWrapper.LobbyControl.LeaveLobby(_eosCore._ILobby, lobby._lobbyID, lobby._localPUID, (ref LeaveLobbyCallbackInfo info) =>
            {
                if (ETC.ErrControl(info.ResultCode, _onLeaveLobby))
                {
                    _onLeaveLobby?.Invoke(info.ResultCode, lobby);
                }
            });
        }
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
    void OnLobbyUpdateReceived(ref LobbyUpdateReceivedCallbackInfo info)
    {
        if (_currentLobby == null) return;
        if(_currentLobby._lobbyID == info.LobbyId)
        {
            _currentLobby.OnLobbyUpdateReceived(info);
        }
    }
    void OnLobbyMemberUpdateReceived(ref LobbyMemberUpdateReceivedCallbackInfo info)
    {
        if (_currentLobby == null) return;
        if (_currentLobby._lobbyID == info.LobbyId)
        {
            _currentLobby.OnLobbyMemberUpdateReceived(info);
        }
    }
    void OnLobbyMemberStatusReceived(ref LobbyMemberStatusReceivedCallbackInfo info)
    {
        if (_currentLobby == null) return;
        if (_currentLobby._lobbyID == info.LobbyId)
        {
            _currentLobby.OnLobbyMemberStatusReceived(info);
            if (info.CurrentStatus == LobbyMemberStatus.Left)
            {
                if (info.TargetUserId.ToString() == _currentLobby._lobbyOwner.ToString())
                {
                    _onLeaveLobby?.Invoke(Result.Success, _currentLobby);
                    _currentLobby = null;
                }
            }
        }

    }
    void OnLeaveLobbyRequested(ref LeaveLobbyRequestedCallbackInfo info)
    {
        LeaveLobby();
    }
    void OnLobbyInviteAccepted(ref LobbyInviteAcceptedCallbackInfo info)
    {
        if (LobbyControl.CopyLobbyDetailsByInviteID(_eosCore._ILobby, info.InviteId, out var details))
        {
            JoinLobby(details);
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
            JoinLobby(details);
        }
    }
    bool IsCurrentJoined(LobbyDetails details)
    {
        if (_currentLobby == null)
        {
            return false;
        }
        else
        {
            if (ETC.ErrControl(LobbyControl.GetLobbyDetailsInfo(details, out var detailsInfo)))
            {
                if (detailsInfo.Value.LobbyId == _currentLobby._lobbyID)
                {
                    return true;
                }
            }
        }
        return false;
    }
    string GenerateLobbyCode()
    {
        /*
         * 공개방 검색에 노출을 줄이고 싶은 경우 로비 코드로 검색하게 해두었음 
         * 로비 코드는 중복될 가능성이 있음
         * 가장 좋은건 알아서 할당해 주는 고유 LobbyID 를 쓰는건데 쓰기에 너무 길다
         */
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string digits = "0123456789";
        string code = "";
        for (int i = 0; i < 5; i++)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                code += digits[UnityEngine.Random.Range(0, digits.Length)];
            }
            else
            {
                code += letters[UnityEngine.Random.Range(0, letters.Length)];
            }
        }
        return code;
    }
    private EOS_Lobby CreateJoinedLobby(string lobbyID)
    {
        _currentLobby = new EOS_Lobby(lobbyID, _localUser.GetLocalPUID());
        _onJoinLobby?.Invoke(Result.Success,_currentLobby);
        return _currentLobby;
    }
    public class EOS_Lobby
    {
        EOS_Core _eosNet;
        EOS_SingleLobbyManager _singleLobbyManager;
        public string _lobbyID { get; private set; }
        public ProductUserId _localPUID { get; private set; }
        public ProductUserId _lobbyOwner { get; private set; }
        public Dictionary<string, Epic.OnlineServices.Lobby.Attribute> _attribute;
        public Dictionary<ProductUserId, EOS_LobbyMember> _members;
        public EOS_Lobby(string lobbyID, ProductUserId localPUID)
        {
            _lobbyID = lobbyID;
            _localPUID = localPUID;
            _members = new Dictionary<ProductUserId, EOS_LobbyMember>();
            _attribute = new Dictionary<string, Epic.OnlineServices.Lobby.Attribute>();
            _eosNet = SingletonMonoBehaviour<EOS_Core>._instance;
            _singleLobbyManager = SingletonMonoBehaviour<EOS_SingleLobbyManager>._instance;
            if (ETC.ErrControl(LobbyControl.CopyLobbyDetailsByLobbyID(_eosNet._ILobby, _lobbyID, _localPUID, out var details)))
            {
                UpdateLobbyOwner(details);
                UpdateLobbyAttribute(details);
                UpdateMembers(details);
                details.Release();
            }
        }

        public bool GetLobbyInfo(out Epic.OnlineServices.Lobby.Attribute attr)
        {
            return _attribute.TryGetValue("LOBBYINFO", out attr);
        }
        public bool GetLobbyType(out Epic.OnlineServices.Lobby.Attribute attr)
        {
            return _attribute.TryGetValue("LOBBYTYPE", out attr);
        }
        public bool GetLobbyCode(out Epic.OnlineServices.Lobby.Attribute attr)
        {
            return _attribute.TryGetValue("LOBBYCODE", out attr);
        }
        
        public void UpdateMembers(LobbyDetails details)
        {
            uint memberCount = EOSWrapper.LobbyControl.GetCurrentMemberCount(details);
            Dictionary<ProductUserId, EOS_LobbyMember> newMembers = new Dictionary<ProductUserId, EOS_LobbyMember>();
            for (uint i = 0; i < memberCount; i++)
            {
                if (EOSWrapper.LobbyControl.GetMemberByIndex(details, i, out var memberPUID))
                {
                    if (_members.TryGetValue(memberPUID, out var member))
                    {
                        newMembers.Add(memberPUID, member);
                        _members.Remove(memberPUID);
                    }
                    else
                    {
                        member = new EOS_LobbyMember(memberPUID);
                        newMembers.TryAdd(memberPUID, member);
                        _singleLobbyManager._onMemberUpdate?.Invoke(member);
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
                    _lobbyOwner = owner;
                }
            }
        }
        public void UpdateLobbyAttribute(LobbyDetails details)
        {
            uint attrCount = EOSWrapper.LobbyControl.GetLobbyAttributeCount(details);
            _attribute.Clear();
            for (uint i = 0; i < attrCount; i++)
            {
                if (ETC.ErrControl(LobbyControl.GetLobbyAttributeByIndex(details, i, out var attr)))
                {

                    if (!_attribute.TryGetValue(attr.Value.Data.Value.Key, out var _))
                    {
                        _attribute.TryAdd(attr.Value.Data.Value.Key, attr.Value);
                        _singleLobbyManager._onLobbyAttributeUpdate?.Invoke(attr.Value);
                    }
                    else
                    {
                        if (EOSWrapper.ETC.Equal(_attribute[attr.Value.Data.Value.Key], attr.Value))
                        {
                            _attribute[attr.Value.Data.Value.Key] = attr.Value;
                            _singleLobbyManager._onLobbyAttributeUpdate?.Invoke(attr.Value);
                        }
                    }
                }
            }
        }
        public void UpdateMembersAttribute(LobbyDetails details, ProductUserId memberPUID)
        {
            if (_members.TryGetValue(memberPUID, out var member))
            {
                uint attrCount = EOSWrapper.LobbyControl.GetMemberAttributeCount(details, memberPUID);
                member._attribute.Clear();
                for (uint i = 0; i < attrCount; i++)
                {
                    if (EOSWrapper.LobbyControl.GetMemberAttributeByIndex(details, memberPUID, i, out var attr))
                    {
                        if (!member._attribute.TryGetValue(attr.Value.Data.Value.Key, out var _))
                        {
                            member._attribute.TryAdd(attr.Value.Data.Value.Key, attr.Value);
                            _singleLobbyManager._onMemberAttributeUpdate?.Invoke(member, attr.Value);
                        }
                        else
                        {
                            if (EOSWrapper.ETC.Equal(member._attribute[attr.Value.Data.Value.Key], attr.Value))
                            {
                                member._attribute[attr.Value.Data.Value.Key] = attr.Value;
                                _singleLobbyManager._onMemberAttributeUpdate?.Invoke(member, attr.Value);
                            }
                        }
                    }
                }
            }
        }
        public void UpdateMemberState(ProductUserId memberPUID, LobbyMemberStatus state)
        {
            if (_members.TryGetValue(memberPUID, out var member))
            {
                if (member._state != state)
                {
                    member.SetState(state);
                    _singleLobbyManager._onMemberStateUpdate?.Invoke(member);
                }
                if (member._state == LobbyMemberStatus.Left)
                {
                    _members.Remove(member._localPUID);
                }
            }
            else if (state == LobbyMemberStatus.Joined)
            {
                member = new EOS_LobbyMember(memberPUID);
                _members.TryAdd(memberPUID, member);
                _singleLobbyManager._onMemberStateUpdate?.Invoke(member);
            }
        }
        public void OnLobbyUpdateReceived(LobbyUpdateReceivedCallbackInfo info)
        {
            if (ETC.ErrControl(LobbyControl.CopyLobbyDetailsByLobbyID(_eosNet._ILobby, _lobbyID, _localPUID, out var details)))
            {
                UpdateLobbyAttribute(details);
                details.Release();
            }
        }
        public void OnLobbyMemberUpdateReceived(LobbyMemberUpdateReceivedCallbackInfo info)
        {
            if (ETC.ErrControl(LobbyControl.CopyLobbyDetailsByLobbyID(_eosNet._ILobby, _lobbyID, _localPUID, out var details)))
            {
                UpdateMembersAttribute(details, info.TargetUserId);
                details.Release();
            }
        }
        public void OnLobbyMemberStatusReceived(LobbyMemberStatusReceivedCallbackInfo info)
        {
            UpdateMemberState(info.TargetUserId, info.CurrentStatus);
        }
    }
}