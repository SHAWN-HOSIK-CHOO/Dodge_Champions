// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Auth
{
    /// <summary>
    /// A structure that contains an ID token.
    /// These structures are created by <see cref="AuthInterface.CopyIdToken" /> and must be passed to <see cref="AuthInterface.Release" /> when finished.
    /// </summary>
    public struct IdToken
    {
        /// <summary>
        /// The Epic Account ID described by the ID token.
        /// Use <see cref="EpicAccountId.FromString" /> to populate this field when validating a received ID token.
        /// </summary>
        public EpicAccountId AccountId { get; set; }

        /// <summary>
        /// The ID token as a Json Web Token (JWT) string.
        /// </summary>
        public Utf8String JsonWebToken { get; set; }

        internal void Set(ref IdTokenInternal other)
        {
            AccountId = other.AccountId;
            JsonWebToken = other.JsonWebToken;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct IdTokenInternal : IGettable<IdToken>, ISettable<IdToken>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_AccountId;
        private System.IntPtr m_JsonWebToken;

        public EpicAccountId AccountId
        {
            get
            {
                EpicAccountId value;
                Helper.Get(m_AccountId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_AccountId);
            }
        }

        public Utf8String JsonWebToken
        {
            get
            {
                Utf8String value;
                Helper.Get(m_JsonWebToken, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_JsonWebToken);
            }
        }

        public void Set(ref IdToken other)
        {
            m_ApiVersion = AuthInterface.CopyidtokenApiLatest;
            AccountId = other.AccountId;
            JsonWebToken = other.JsonWebToken;
        }

        public void Set(ref IdToken? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = AuthInterface.CopyidtokenApiLatest;
                AccountId = other.Value.AccountId;
                JsonWebToken = other.Value.JsonWebToken;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_AccountId);
            Helper.Dispose(ref m_JsonWebToken);
        }

        public void Get(out IdToken output)
        {
            output = new IdToken();
            output.Set(ref this);
        }
    }
}