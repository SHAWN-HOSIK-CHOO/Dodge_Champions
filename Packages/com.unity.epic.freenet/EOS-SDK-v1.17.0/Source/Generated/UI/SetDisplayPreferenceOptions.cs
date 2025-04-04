// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.UI
{
    /// <summary>
    /// Input parameters for the <see cref="UIInterface.SetDisplayPreference" /> function.
    /// </summary>
    public struct SetDisplayPreferenceOptions
    {
        /// <summary>
        /// Preference for notification pop-up locations.
        /// </summary>
        public NotificationLocation NotificationLocation { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct SetDisplayPreferenceOptionsInternal : ISettable<SetDisplayPreferenceOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private NotificationLocation m_NotificationLocation;

        public NotificationLocation NotificationLocation
        {
            set
            {
                m_NotificationLocation = value;
            }
        }

        public void Set(ref SetDisplayPreferenceOptions other)
        {
            m_ApiVersion = UIInterface.SetdisplaypreferenceApiLatest;
            NotificationLocation = other.NotificationLocation;
        }

        public void Set(ref SetDisplayPreferenceOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = UIInterface.SetdisplaypreferenceApiLatest;
                NotificationLocation = other.Value.NotificationLocation;
            }
        }

        public void Dispose()
        {
        }
    }
}