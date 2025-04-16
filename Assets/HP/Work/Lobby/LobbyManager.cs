using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using System;
using System.Collections.Generic;
namespace HP
{
    public class LobbyManager : EOS_LobbyManager
    {
        public static new LobbyManager Instance => (EOS_LobbyManager.Instance as LobbyManager);
        public enum LobbySecurityType
        {
            Public,
            Protected,
        }
        public void CreateLobby(string mode, string name, uint maxMember, LobbySecurityType securityType, string code, Action<Result, EOS_Lobby> onComplete = null)
        {
            if (!_Initialied) return;
            Epic.OnlineServices.Lobby.AttributeData[] searchParams = new Epic.OnlineServices.Lobby.AttributeData[7]
            {
                new AttributeData { Key = "LOBBYTYPE", Value = "Lobby"},
                new AttributeData { Key = "SOCKET", Value = new Random().Next(10000, 100000).ToString("D5")},
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
                BucketId = "KR",
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
            if (EOSWrapper.ETC.ErrControl(_Initialied, onComplete))
            {
                List<(Epic.OnlineServices.Lobby.AttributeData, ComparisonOp)> searchParams = new List<(AttributeData, ComparisonOp)>();
                searchParams.Add((new AttributeData { Key = "LOBBYTYPE", Value = "Lobby" }, ComparisonOp.Equal));
                FindLobby(findNum, searchParams, onComplete);
            }
        }
        public override EOS_Lobby CreateLobby(EOS_Core eosCore, EOS_LobbyManager lobbyManager, string lobbyID)
        {
            return new Lobby(eosCore, lobbyManager, lobbyID, _localUser._localPUID);
        }
    }
}