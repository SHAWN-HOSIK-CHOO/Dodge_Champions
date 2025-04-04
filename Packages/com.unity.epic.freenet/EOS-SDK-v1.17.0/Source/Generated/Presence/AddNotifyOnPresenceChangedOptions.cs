// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Presence
{
    /// <summary>
    /// Data for the <see cref="PresenceInterface.AddNotifyOnPresenceChanged" /> function.
    /// </summary>
    public struct AddNotifyOnPresenceChangedOptions
    {
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct AddNotifyOnPresenceChangedOptionsInternal : ISettable<AddNotifyOnPresenceChangedOptions>, System.IDisposable
    {
        private int m_ApiVersion;

        public void Set(ref AddNotifyOnPresenceChangedOptions other)
        {
            m_ApiVersion = PresenceInterface.AddnotifyonpresencechangedApiLatest;
        }

        public void Set(ref AddNotifyOnPresenceChangedOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = PresenceInterface.AddnotifyonpresencechangedApiLatest;
            }
        }

        public void Dispose()
        {
        }
    }
}