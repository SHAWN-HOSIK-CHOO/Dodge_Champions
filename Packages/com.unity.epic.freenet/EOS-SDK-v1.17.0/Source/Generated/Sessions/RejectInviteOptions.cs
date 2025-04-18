// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sessions
{
    /// <summary>
    /// Input parameters for the <see cref="SessionsInterface.RejectInvite" /> function.
    /// </summary>
    public struct RejectInviteOptions
    {
        /// <summary>
        /// The Product User ID of the local user rejecting the invitation
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The invite ID to reject
        /// </summary>
        public Utf8String InviteId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct RejectInviteOptionsInternal : ISettable<RejectInviteOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_InviteId;

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String InviteId
        {
            set
            {
                Helper.Set(value, ref m_InviteId);
            }
        }

        public void Set(ref RejectInviteOptions other)
        {
            m_ApiVersion = SessionsInterface.RejectinviteApiLatest;
            LocalUserId = other.LocalUserId;
            InviteId = other.InviteId;
        }

        public void Set(ref RejectInviteOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = SessionsInterface.RejectinviteApiLatest;
                LocalUserId = other.Value.LocalUserId;
                InviteId = other.Value.InviteId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_InviteId);
        }
    }
}