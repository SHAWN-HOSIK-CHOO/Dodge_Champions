// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.UI
{
    /// <summary>
    /// Parameters for the <see cref="UIInterface.ShowBlockPlayer" /> function.
    /// </summary>
    public struct ShowBlockPlayerOptions
    {
        /// <summary>
        /// The Epic Online Services Account ID of the user who is requesting the Block.
        /// </summary>
        public EpicAccountId LocalUserId { get; set; }

        /// <summary>
        /// The Epic Online Services Account ID of the user whose is being Blocked.
        /// </summary>
        public EpicAccountId TargetUserId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct ShowBlockPlayerOptionsInternal : ISettable<ShowBlockPlayerOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_TargetUserId;

        public EpicAccountId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public EpicAccountId TargetUserId
        {
            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public void Set(ref ShowBlockPlayerOptions other)
        {
            m_ApiVersion = UIInterface.ShowblockplayerApiLatest;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
        }

        public void Set(ref ShowBlockPlayerOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = UIInterface.ShowblockplayerApiLatest;
                LocalUserId = other.Value.LocalUserId;
                TargetUserId = other.Value.TargetUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_TargetUserId);
        }
    }
}