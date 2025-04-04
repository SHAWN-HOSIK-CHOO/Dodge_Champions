// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCAudio
{
    /// <summary>
    /// This struct is passed in with a call to <see cref="RTCAudioInterface.UpdateSendingVolume" />
    /// </summary>
    public struct UpdateSendingVolumeOptions
    {
        /// <summary>
        /// The Product User ID of the user trying to request this operation.
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The room this settings should be applied on.
        /// </summary>
        public Utf8String RoomName { get; set; }

        /// <summary>
        /// The volume to be set for sent audio (range 0.0 to 100.0). Volume 50 means that the audio volume is not modified
        /// </summary>
        public float Volume { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct UpdateSendingVolumeOptionsInternal : ISettable<UpdateSendingVolumeOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_RoomName;
        private float m_Volume;

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String RoomName
        {
            set
            {
                Helper.Set(value, ref m_RoomName);
            }
        }

        public float Volume
        {
            set
            {
                m_Volume = value;
            }
        }

        public void Set(ref UpdateSendingVolumeOptions other)
        {
            m_ApiVersion = RTCAudioInterface.UpdatesendingvolumeApiLatest;
            LocalUserId = other.LocalUserId;
            RoomName = other.RoomName;
            Volume = other.Volume;
        }

        public void Set(ref UpdateSendingVolumeOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = RTCAudioInterface.UpdatesendingvolumeApiLatest;
                LocalUserId = other.Value.LocalUserId;
                RoomName = other.Value.RoomName;
                Volume = other.Value.Volume;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_RoomName);
        }
    }
}