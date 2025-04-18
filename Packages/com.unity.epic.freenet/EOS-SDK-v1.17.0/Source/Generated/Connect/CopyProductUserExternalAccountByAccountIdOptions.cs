// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
    /// <summary>
    /// Input parameters for the <see cref="ConnectInterface.CopyProductUserExternalAccountByAccountId" /> function.
    /// </summary>
    public struct CopyProductUserExternalAccountByAccountIdOptions
    {
        /// <summary>
        /// The Product User ID to look for when copying external account info from the cache.
        /// </summary>
        public ProductUserId TargetUserId { get; set; }

        /// <summary>
        /// External auth service account ID to look for when copying external account info from the cache.
        /// </summary>
        public Utf8String AccountId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyProductUserExternalAccountByAccountIdOptionsInternal : ISettable<CopyProductUserExternalAccountByAccountIdOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_TargetUserId;
        private System.IntPtr m_AccountId;

        public ProductUserId TargetUserId
        {
            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public Utf8String AccountId
        {
            set
            {
                Helper.Set(value, ref m_AccountId);
            }
        }

        public void Set(ref CopyProductUserExternalAccountByAccountIdOptions other)
        {
            m_ApiVersion = ConnectInterface.CopyproductuserexternalaccountbyaccountidApiLatest;
            TargetUserId = other.TargetUserId;
            AccountId = other.AccountId;
        }

        public void Set(ref CopyProductUserExternalAccountByAccountIdOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = ConnectInterface.CopyproductuserexternalaccountbyaccountidApiLatest;
                TargetUserId = other.Value.TargetUserId;
                AccountId = other.Value.AccountId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_TargetUserId);
            Helper.Dispose(ref m_AccountId);
        }
    }
}