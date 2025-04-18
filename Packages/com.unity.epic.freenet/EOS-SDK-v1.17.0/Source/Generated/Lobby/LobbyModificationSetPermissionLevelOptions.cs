// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyModification.SetPermissionLevel" /> function.
    /// </summary>
    public struct LobbyModificationSetPermissionLevelOptions
    {
        /// <summary>
        /// Permission level of the lobby
        /// </summary>
        public LobbyPermissionLevel PermissionLevel { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LobbyModificationSetPermissionLevelOptionsInternal : ISettable<LobbyModificationSetPermissionLevelOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private LobbyPermissionLevel m_PermissionLevel;

        public LobbyPermissionLevel PermissionLevel
        {
            set
            {
                m_PermissionLevel = value;
            }
        }

        public void Set(ref LobbyModificationSetPermissionLevelOptions other)
        {
            m_ApiVersion = LobbyModification.LobbymodificationSetpermissionlevelApiLatest;
            PermissionLevel = other.PermissionLevel;
        }

        public void Set(ref LobbyModificationSetPermissionLevelOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyModification.LobbymodificationSetpermissionlevelApiLatest;
                PermissionLevel = other.Value.PermissionLevel;
            }
        }

        public void Dispose()
        {
        }
    }
}