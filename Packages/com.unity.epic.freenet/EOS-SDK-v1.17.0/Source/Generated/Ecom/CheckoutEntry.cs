// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
    /// <summary>
    /// Contains information about a request to purchase a single offer. This structure is set as part
    /// of the <see cref="CheckoutOptions" /> structure.
    /// </summary>
    public struct CheckoutEntry
    {
        /// <summary>
        /// The ID of the offer to purchase
        /// </summary>
        public Utf8String OfferId { get; set; }

        internal void Set(ref CheckoutEntryInternal other)
        {
            OfferId = other.OfferId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CheckoutEntryInternal : IGettable<CheckoutEntry>, ISettable<CheckoutEntry>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_OfferId;

        public Utf8String OfferId
        {
            get
            {
                Utf8String value;
                Helper.Get(m_OfferId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_OfferId);
            }
        }

        public void Set(ref CheckoutEntry other)
        {
            m_ApiVersion = EcomInterface.CheckoutentryApiLatest;
            OfferId = other.OfferId;
        }

        public void Set(ref CheckoutEntry? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = EcomInterface.CheckoutentryApiLatest;
                OfferId = other.Value.OfferId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_OfferId);
        }

        public void Get(out CheckoutEntry output)
        {
            output = new CheckoutEntry();
            output.Set(ref this);
        }
    }
}