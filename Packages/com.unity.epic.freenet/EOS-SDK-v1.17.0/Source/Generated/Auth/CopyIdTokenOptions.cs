// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Auth
{
    /// <summary>
    /// Input parameters for the <see cref="AuthInterface.CopyIdToken" /> function.
    /// </summary>
    public struct CopyIdTokenOptions
    {
        /// <summary>
        /// The Epic Account ID of the user being queried.
        /// </summary>
        public EpicAccountId AccountId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyIdTokenOptionsInternal : ISettable<CopyIdTokenOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_AccountId;

        public EpicAccountId AccountId
        {
            set
            {
                Helper.Set(value, ref m_AccountId);
            }
        }

        public void Set(ref CopyIdTokenOptions other)
        {
            m_ApiVersion = AuthInterface.CopyidtokenApiLatest;
            AccountId = other.AccountId;
        }

        public void Set(ref CopyIdTokenOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = AuthInterface.CopyidtokenApiLatest;
                AccountId = other.Value.AccountId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_AccountId);
        }
    }
}