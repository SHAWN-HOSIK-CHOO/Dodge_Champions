// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyDetails.GetMemberByIndex" /> function.
    /// </summary>
    public struct LobbyDetailsGetMemberByIndexOptions
    {
        /// <summary>
        /// Index of the member to retrieve
        /// </summary>
        public uint MemberIndex { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LobbyDetailsGetMemberByIndexOptionsInternal : ISettable<LobbyDetailsGetMemberByIndexOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private uint m_MemberIndex;

        public uint MemberIndex
        {
            set
            {
                m_MemberIndex = value;
            }
        }

        public void Set(ref LobbyDetailsGetMemberByIndexOptions other)
        {
            m_ApiVersion = LobbyDetails.LobbydetailsGetmemberbyindexApiLatest;
            MemberIndex = other.MemberIndex;
        }

        public void Set(ref LobbyDetailsGetMemberByIndexOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyDetails.LobbydetailsGetmemberbyindexApiLatest;
                MemberIndex = other.Value.MemberIndex;
            }
        }

        public void Dispose()
        {
        }
    }
}