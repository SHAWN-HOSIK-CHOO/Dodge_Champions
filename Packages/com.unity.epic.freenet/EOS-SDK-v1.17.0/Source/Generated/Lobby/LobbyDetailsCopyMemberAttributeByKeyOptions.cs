// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyDetails.CopyMemberAttributeByKey" /> function.
    /// </summary>
    public struct LobbyDetailsCopyMemberAttributeByKeyOptions
    {
        /// <summary>
        /// The Product User ID of the lobby member
        /// </summary>
        public ProductUserId TargetUserId { get; set; }

        /// <summary>
        /// Name of the attribute to copy
        /// </summary>
        public Utf8String AttrKey { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LobbyDetailsCopyMemberAttributeByKeyOptionsInternal : ISettable<LobbyDetailsCopyMemberAttributeByKeyOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_TargetUserId;
        private System.IntPtr m_AttrKey;

        public ProductUserId TargetUserId
        {
            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public Utf8String AttrKey
        {
            set
            {
                Helper.Set(value, ref m_AttrKey);
            }
        }

        public void Set(ref LobbyDetailsCopyMemberAttributeByKeyOptions other)
        {
            m_ApiVersion = LobbyDetails.LobbydetailsCopymemberattributebykeyApiLatest;
            TargetUserId = other.TargetUserId;
            AttrKey = other.AttrKey;
        }

        public void Set(ref LobbyDetailsCopyMemberAttributeByKeyOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyDetails.LobbydetailsCopymemberattributebykeyApiLatest;
                TargetUserId = other.Value.TargetUserId;
                AttrKey = other.Value.AttrKey;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_TargetUserId);
            Helper.Dispose(ref m_AttrKey);
        }
    }
}