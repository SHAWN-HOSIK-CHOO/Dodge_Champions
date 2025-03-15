using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EOSWrapper;

public class LobbyManager : EOS_LobbyManager
{
    public enum LobbySecurityType
    {
        Public,
        Protected,
    }

    private IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        base.Init(FreeNet._instance);
    }

    public void CreateLobby(string mode, string name, uint maxMember, LobbySecurityType securityType, string code, Action<Result, EOS_Lobby> onComplete = null)
    {
        Epic.OnlineServices.Lobby.AttributeData[] searchParams = new Epic.OnlineServices.Lobby.AttributeData[6]
        { 
            new AttributeData { Key = "SOCKET", Value = "Lobby"},
            new AttributeData { Key = "OWNERNAME", Value = _localUser._localUserInfo.Value.DisplayName},
            new AttributeData{ Key = "MODE", Value = mode},
            new AttributeData { Key = "LOBBYNAME", Value = name},
            new AttributeData{ Key = "LOBBYSECURITY", Value = securityType.ToString()},
            new AttributeData { Key = "LOBBYCODE", Value = code},
        };
        var options = new CreateLobbyOptions()
        {
            LocalUserId = _localUser._localPUID._PUID,
            MaxLobbyMembers = maxMember,
            PermissionLevel = LobbyPermissionLevel.Publicadvertised,
            BucketId = securityType.ToString(),
            PresenceEnabled = true,
            AllowInvites = true,
            DisableHostMigration = true,
            EnableRTCRoom = false,
            EnableJoinById = true,
        };
        base.CreateLobby(options, searchParams, onComplete);
    }
    public void FindLobby(uint findNum, Action<Result, List<EOS_LobbySearchResult>> onComplete = null)
    {
        List<Epic.OnlineServices.Lobby.AttributeData> searchParams = new List<AttributeData>();
        searchParams.Add(new AttributeData { Key = "SOCKET", Value = "Lobby" });
        FindLobby(findNum, searchParams, onComplete);
    }
    public override EOS_Lobby CreateLobby(EOS_Core eosCore, EOS_LobbyManager lobbyManager, string lobbyID)
    {
        return new Lobby(eosCore, lobbyManager, lobbyID, _localUser._localPUID);
    }
}