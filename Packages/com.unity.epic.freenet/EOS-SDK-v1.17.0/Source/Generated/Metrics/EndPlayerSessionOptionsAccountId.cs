// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Metrics
{
    public struct EndPlayerSessionOptionsAccountId
    {
        private MetricsAccountIdType m_AccountIdType;
        private EpicAccountId m_Epic;
        private Utf8String m_External;

        /// <summary>
        /// The Account ID type that is set in the union.
        /// </summary>
        public MetricsAccountIdType AccountIdType
        {
            get
            {
                return m_AccountIdType;
            }

            private set
            {
                m_AccountIdType = value;
            }
        }

        /// <summary>
        /// An Epic Account ID. Set this field when AccountIdType is set to <see cref="MetricsAccountIdType.Epic" />.
        /// </summary>
        public EpicAccountId Epic
        {
            get
            {
                EpicAccountId value;
                Helper.Get(m_Epic, out value, m_AccountIdType, MetricsAccountIdType.Epic);
                return value;
            }

            set
            {
                Helper.Set<EpicAccountId, Metrics.MetricsAccountIdType>(value, ref m_Epic, MetricsAccountIdType.Epic, ref m_AccountIdType);
            }
        }

        /// <summary>
        /// An Account ID for another service. Set this field when AccountIdType is set to <see cref="MetricsAccountIdType.External" />.
        /// </summary>
        public Utf8String External
        {
            get
            {
                Utf8String value;
                Helper.Get(m_External, out value, m_AccountIdType, MetricsAccountIdType.External);
                return value;
            }

            set
            {
                Helper.Set<Utf8String, Metrics.MetricsAccountIdType>(value, ref m_External, MetricsAccountIdType.External, ref m_AccountIdType);
            }
        }

        public static implicit operator EndPlayerSessionOptionsAccountId(EpicAccountId value)
        {
            return new EndPlayerSessionOptionsAccountId() { Epic = value };
        }

        public static implicit operator EndPlayerSessionOptionsAccountId(Utf8String value)
        {
            return new EndPlayerSessionOptionsAccountId() { External = value };
        }

        public static implicit operator EndPlayerSessionOptionsAccountId(string value)
        {
            return new EndPlayerSessionOptionsAccountId() { External = value };
        }

        internal void Set(ref EndPlayerSessionOptionsAccountIdInternal other)
        {
            Epic = other.Epic;
            External = other.External;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 4)]
    internal struct EndPlayerSessionOptionsAccountIdInternal : IGettable<EndPlayerSessionOptionsAccountId>, ISettable<EndPlayerSessionOptionsAccountId>, System.IDisposable
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        private MetricsAccountIdType m_AccountIdType;
        [System.Runtime.InteropServices.FieldOffset(4)]
        private System.IntPtr m_Epic;
        [System.Runtime.InteropServices.FieldOffset(4)]
        private System.IntPtr m_External;

        public EpicAccountId Epic
        {
            get
            {
                EpicAccountId value;
                Helper.Get(m_Epic, out value, m_AccountIdType, MetricsAccountIdType.Epic);
                return value;
            }

            set
            {
                Helper.Set<Metrics.MetricsAccountIdType>(value, ref m_Epic, MetricsAccountIdType.Epic, ref m_AccountIdType, this);
            }
        }

        public Utf8String External
        {
            get
            {
                Utf8String value;
                Helper.Get(m_External, out value, m_AccountIdType, MetricsAccountIdType.External);
                return value;
            }

            set
            {
                Helper.Set<Metrics.MetricsAccountIdType>(value, ref m_External, MetricsAccountIdType.External, ref m_AccountIdType, this);
            }
        }

        public void Set(ref EndPlayerSessionOptionsAccountId other)
        {
            Epic = other.Epic;
            External = other.External;
        }

        public void Set(ref EndPlayerSessionOptionsAccountId? other)
        {
            if (other.HasValue)
            {
                Epic = other.Value.Epic;
                External = other.Value.External;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_Epic);
            Helper.Dispose(ref m_External, m_AccountIdType, MetricsAccountIdType.External);
        }

        public void Get(out EndPlayerSessionOptionsAccountId output)
        {
            output = new EndPlayerSessionOptionsAccountId();
            output.Set(ref this);
        }
    }
}