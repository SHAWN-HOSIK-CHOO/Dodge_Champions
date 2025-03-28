// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
    /// <summary>
    /// Input parameters for the <see cref="ConnectInterface.CopyIdToken" /> function.
    /// </summary>
    public struct CopyIdTokenOptions
    {
        /// <summary>
        /// The local Product User ID whose ID token should be copied.
        /// </summary>
        public ProductUserId LocalUserId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyIdTokenOptionsInternal : ISettable<CopyIdTokenOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public void Set(ref CopyIdTokenOptions other)
        {
            m_ApiVersion = ConnectInterface.CopyidtokenApiLatest;
            LocalUserId = other.LocalUserId;
        }

        public void Set(ref CopyIdTokenOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = ConnectInterface.CopyidtokenApiLatest;
                LocalUserId = other.Value.LocalUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
        }
    }
}