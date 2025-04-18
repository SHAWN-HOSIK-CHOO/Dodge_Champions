// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sanctions
{
    /// <summary>
    /// Contains information about a single player sanction.
    /// </summary>
    public struct PlayerSanction
    {
        /// <summary>
        /// The POSIX timestamp when the sanction was placed
        /// </summary>
        public long TimePlaced { get; set; }

        /// <summary>
        /// The action associated with this sanction
        /// </summary>
        public Utf8String Action { get; set; }

        /// <summary>
        /// The POSIX timestamp when the sanction will expire. If the sanction is permanent, this will be 0.
        /// </summary>
        public long TimeExpires { get; set; }

        /// <summary>
        /// A unique identifier for this specific sanction
        /// </summary>
        public Utf8String ReferenceId { get; set; }

        internal void Set(ref PlayerSanctionInternal other)
        {
            TimePlaced = other.TimePlaced;
            Action = other.Action;
            TimeExpires = other.TimeExpires;
            ReferenceId = other.ReferenceId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct PlayerSanctionInternal : IGettable<PlayerSanction>, ISettable<PlayerSanction>, System.IDisposable
    {
        private int m_ApiVersion;
        private long m_TimePlaced;
        private System.IntPtr m_Action;
        private long m_TimeExpires;
        private System.IntPtr m_ReferenceId;

        public long TimePlaced
        {
            get
            {
                return m_TimePlaced;
            }

            set
            {
                m_TimePlaced = value;
            }
        }

        public Utf8String Action
        {
            get
            {
                Utf8String value;
                Helper.Get(m_Action, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_Action);
            }
        }

        public long TimeExpires
        {
            get
            {
                return m_TimeExpires;
            }

            set
            {
                m_TimeExpires = value;
            }
        }

        public Utf8String ReferenceId
        {
            get
            {
                Utf8String value;
                Helper.Get(m_ReferenceId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_ReferenceId);
            }
        }

        public void Set(ref PlayerSanction other)
        {
            m_ApiVersion = SanctionsInterface.PlayersanctionApiLatest;
            TimePlaced = other.TimePlaced;
            Action = other.Action;
            TimeExpires = other.TimeExpires;
            ReferenceId = other.ReferenceId;
        }

        public void Set(ref PlayerSanction? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = SanctionsInterface.PlayersanctionApiLatest;
                TimePlaced = other.Value.TimePlaced;
                Action = other.Value.Action;
                TimeExpires = other.Value.TimeExpires;
                ReferenceId = other.Value.ReferenceId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_Action);
            Helper.Dispose(ref m_ReferenceId);
        }

        public void Get(out PlayerSanction output)
        {
            output = new PlayerSanction();
            output.Set(ref this);
        }
    }
}