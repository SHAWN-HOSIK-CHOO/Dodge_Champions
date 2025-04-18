// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCAudio
{
    /// <summary>
    /// This struct is used to call <see cref="RTCAudioInterface.SetAudioOutputSettings" />.
    /// </summary>
    public struct SetAudioOutputSettingsOptions
    {
        /// <summary>
        /// The Product User ID of the user who initiated this request.
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The device Id to be used for this user. Pass <see langword="null" /> or empty string to use default output device.
        /// 
        /// If the device ID is invalid, the default device will be used instead.
        /// Despite of this fact, that device ID will be stored and the library will try to move on it when a device pool is being changed.
        /// 
        /// The actual hardware audio device usage depends on the current payload and optimized not to use it
        /// when generated audio frames cannot be processed by someone else based on a scope of rules (For instance, when a client is alone in a room).
        /// <seealso cref="RTCAudioInterface.AddNotifyAudioDevicesChanged" />
        /// </summary>
        public Utf8String DeviceId { get; set; }

        /// <summary>
        /// The volume to be used for all rooms of this user (range 0.0 to 100.0).
        /// 
        /// Volume 50.0 means that the audio volume is not modified and stays in its source value.
        /// </summary>
        public float Volume { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct SetAudioOutputSettingsOptionsInternal : ISettable<SetAudioOutputSettingsOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_DeviceId;
        private float m_Volume;

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String DeviceId
        {
            set
            {
                Helper.Set(value, ref m_DeviceId);
            }
        }

        public float Volume
        {
            set
            {
                m_Volume = value;
            }
        }

        public void Set(ref SetAudioOutputSettingsOptions other)
        {
            m_ApiVersion = RTCAudioInterface.SetaudiooutputsettingsApiLatest;
            LocalUserId = other.LocalUserId;
            DeviceId = other.DeviceId;
            Volume = other.Volume;
        }

        public void Set(ref SetAudioOutputSettingsOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = RTCAudioInterface.SetaudiooutputsettingsApiLatest;
                LocalUserId = other.Value.LocalUserId;
                DeviceId = other.Value.DeviceId;
                Volume = other.Value.Volume;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_DeviceId);
        }
    }
}