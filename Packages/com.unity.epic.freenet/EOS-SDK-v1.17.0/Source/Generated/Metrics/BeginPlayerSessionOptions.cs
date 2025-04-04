// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Metrics
{
    /// <summary>
    /// BeginPlayerSession.
    /// </summary>
    public struct BeginPlayerSessionOptions
    {
        public BeginPlayerSessionOptionsAccountId AccountId { get; set; }

        /// <summary>
        /// The in-game display name for the user as UTF-8 string.
        /// </summary>
        public Utf8String DisplayName { get; set; }

        /// <summary>
        /// The user's game controller type.
        /// </summary>
        public UserControllerType ControllerType { get; set; }

        /// <summary>
        /// IP address of the game server hosting the game session. For a localhost session, set to <see langword="null" />.
        /// 
        /// @details Must be in either one of the following IPv4 or IPv6 string formats:
        /// "127.0.0.1".
        /// "1200:0000:AB00:1234:0000:2552:7777:1313".
        /// If both IPv4 and IPv6 addresses are available, use the IPv6 address.
        /// </summary>
        public Utf8String ServerIp { get; set; }

        /// <summary>
        /// Optional, application-defined custom match session identifier. If the identifier is not used, set to <see langword="null" />.
        /// 
        /// @details The game can tag each game session with a custom session match identifier,
        /// which will be shown in the Played Sessions listing at the user profile dashboard.
        /// </summary>
        public Utf8String GameSessionId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct BeginPlayerSessionOptionsInternal : ISettable<BeginPlayerSessionOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private BeginPlayerSessionOptionsAccountIdInternal m_AccountId;
        private System.IntPtr m_DisplayName;
        private UserControllerType m_ControllerType;
        private System.IntPtr m_ServerIp;
        private System.IntPtr m_GameSessionId;

        public BeginPlayerSessionOptionsAccountId AccountId
        {
            set
            {
                Helper.Set(ref value, ref m_AccountId);
            }
        }

        public Utf8String DisplayName
        {
            set
            {
                Helper.Set(value, ref m_DisplayName);
            }
        }

        public UserControllerType ControllerType
        {
            set
            {
                m_ControllerType = value;
            }
        }

        public Utf8String ServerIp
        {
            set
            {
                Helper.Set(value, ref m_ServerIp);
            }
        }

        public Utf8String GameSessionId
        {
            set
            {
                Helper.Set(value, ref m_GameSessionId);
            }
        }

        public void Set(ref BeginPlayerSessionOptions other)
        {
            m_ApiVersion = MetricsInterface.BeginplayersessionApiLatest;
            AccountId = other.AccountId;
            DisplayName = other.DisplayName;
            ControllerType = other.ControllerType;
            ServerIp = other.ServerIp;
            GameSessionId = other.GameSessionId;
        }

        public void Set(ref BeginPlayerSessionOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = MetricsInterface.BeginplayersessionApiLatest;
                AccountId = other.Value.AccountId;
                DisplayName = other.Value.DisplayName;
                ControllerType = other.Value.ControllerType;
                ServerIp = other.Value.ServerIp;
                GameSessionId = other.Value.GameSessionId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_AccountId);
            Helper.Dispose(ref m_DisplayName);
            Helper.Dispose(ref m_ServerIp);
            Helper.Dispose(ref m_GameSessionId);
        }
    }
}