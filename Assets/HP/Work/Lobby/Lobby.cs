using Epic.OnlineServices.Lobby;
using System;
using System.Collections.Generic;
using static EOSWrapper;
using static LobbyManager;

public class Lobby : EOS_Lobby
{
    public Lobby(EOS_Core eosCore, EOS_LobbyManager lobbyManager, string lobbyID, EOSWrapper.ETC.PUID localPUID)
        : base(eosCore, lobbyManager, lobbyID, localPUID)
    {
        var result = EOSWrapper.LobbyControl.GetLobbyModification(FreeNet.Instance._eosCore._ILobby, lobbyID, _localPUID._PUID, out var modification);
        if (result == Epic.OnlineServices.Result.Success)
        {
            EOSWrapper.LobbyControl.SetModificationAddMemberAttribute(modification, new Epic.OnlineServices.Lobby.Attribute()
            {
                Visibility = LobbyAttributeVisibility.Public,
                Data = new AttributeData { Key = "DISPLAYNAME", Value = FreeNet.Instance._localUser._localUserInfo.Value.DisplayName }
            });
        }
    }
    public override EOS_LobbyMember CreateLobbyMember(EOS_Lobby lobby, ETC.PUID puid)
    {
        return new LobbyMember(lobby, puid);
    }
}

public static class LobbyAttributeExtenstion
{
    public static bool GetLobbySecurity(this Dictionary<string, Epic.OnlineServices.Lobby.Attribute> attribute, out LobbySecurityType type)
    {
        type = LobbySecurityType.Public;
        if (attribute.TryGetValue("LOBBYSECURITY", out Epic.OnlineServices.Lobby.Attribute attr))
        {
            string typeAsString = attr.Data.Value.Value.AsUtf8;
            type = (LobbySecurityType)Enum.Parse(typeof(LobbySecurityType), typeAsString);
            return true;
        }
        return false;
    }
    public static bool GetLobbyMode(this Dictionary<string, Epic.OnlineServices.Lobby.Attribute> attribute, out string mode)
    {
        mode = null;
        if (attribute.TryGetValue("MODE", out Epic.OnlineServices.Lobby.Attribute attr))
        {
            mode = attr.Data.Value.Value.AsUtf8;
            return true;
        }
        return false;
    }
    public static bool GetOwner(this Dictionary<string, Epic.OnlineServices.Lobby.Attribute> attribute, out string name)
    {
        name = null;
        if (attribute.TryGetValue("OWNERNAME", out Epic.OnlineServices.Lobby.Attribute attr))
        {
            name = attr.Data.Value.Value.AsUtf8;
            return true;
        }
        return false;
    }
    public static bool GetLobbyName(this Dictionary<string, Epic.OnlineServices.Lobby.Attribute> attribute, out string name)
    {
        name = null;
        if (attribute.TryGetValue("LOBBYNAME", out Epic.OnlineServices.Lobby.Attribute attr))
        {
            name = attr.Data.Value.Value.AsUtf8;
            return true;
        }
        return false;
    }
    public static bool GetLobbySocket(this Dictionary<string, Epic.OnlineServices.Lobby.Attribute> attribute, out string socket)
    {
        socket = null;
        if (attribute.TryGetValue("SOCKET", out Epic.OnlineServices.Lobby.Attribute attr))
        {
            socket = attr.Data.Value.Value.AsUtf8;
            return true;
        }
        return false;
    }
    public static bool GetLobbyCode(this Dictionary<string, Epic.OnlineServices.Lobby.Attribute> attribute, out string code)
    {
        code = null;
        if (attribute.TryGetValue("LOBBYCODE", out Epic.OnlineServices.Lobby.Attribute attr))
        {
            code = attr.Data.Value.Value.AsUtf8;
            return true;
        }
        return false;
    }
}