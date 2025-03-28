// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
    /// <summary>
    /// Contains information about a single release within the catalog. Instances of this structure are
    /// created by <see cref="EcomInterface.CopyItemReleaseByIndex" />. They must be passed to <see cref="EcomInterface.Release" />.
    /// </summary>
    public struct CatalogRelease
    {
        /// <summary>
        /// A list of compatible APP IDs
        /// </summary>
        public Utf8String[] CompatibleAppIds { get; set; }

        /// <summary>
        /// A list of compatible Platforms
        /// </summary>
        public Utf8String[] CompatiblePlatforms { get; set; }

        /// <summary>
        /// Release note for compatible versions
        /// </summary>
        public Utf8String ReleaseNote { get; set; }

        internal void Set(ref CatalogReleaseInternal other)
        {
            CompatibleAppIds = other.CompatibleAppIds;
            CompatiblePlatforms = other.CompatiblePlatforms;
            ReleaseNote = other.ReleaseNote;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CatalogReleaseInternal : IGettable<CatalogRelease>, ISettable<CatalogRelease>, System.IDisposable
    {
        private int m_ApiVersion;
        private uint m_CompatibleAppIdCount;
        private System.IntPtr m_CompatibleAppIds;
        private uint m_CompatiblePlatformCount;
        private System.IntPtr m_CompatiblePlatforms;
        private System.IntPtr m_ReleaseNote;

        public Utf8String[] CompatibleAppIds
        {
            get
            {
                Utf8String[] value;
                Helper.Get(m_CompatibleAppIds, out value, m_CompatibleAppIdCount, true);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_CompatibleAppIds, true, out m_CompatibleAppIdCount);
            }
        }

        public Utf8String[] CompatiblePlatforms
        {
            get
            {
                Utf8String[] value;
                Helper.Get(m_CompatiblePlatforms, out value, m_CompatiblePlatformCount, true);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_CompatiblePlatforms, true, out m_CompatiblePlatformCount);
            }
        }

        public Utf8String ReleaseNote
        {
            get
            {
                Utf8String value;
                Helper.Get(m_ReleaseNote, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_ReleaseNote);
            }
        }

        public void Set(ref CatalogRelease other)
        {
            m_ApiVersion = EcomInterface.CatalogreleaseApiLatest;
            CompatibleAppIds = other.CompatibleAppIds;
            CompatiblePlatforms = other.CompatiblePlatforms;
            ReleaseNote = other.ReleaseNote;
        }

        public void Set(ref CatalogRelease? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = EcomInterface.CatalogreleaseApiLatest;
                CompatibleAppIds = other.Value.CompatibleAppIds;
                CompatiblePlatforms = other.Value.CompatiblePlatforms;
                ReleaseNote = other.Value.ReleaseNote;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_CompatibleAppIds);
            Helper.Dispose(ref m_CompatiblePlatforms);
            Helper.Dispose(ref m_ReleaseNote);
        }

        public void Get(out CatalogRelease output)
        {
            output = new CatalogRelease();
            output.Set(ref this);
        }
    }
}