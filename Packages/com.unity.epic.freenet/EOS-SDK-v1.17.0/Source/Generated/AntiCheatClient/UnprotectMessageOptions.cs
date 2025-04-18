// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.AntiCheatClient
{
    public struct UnprotectMessageOptions
    {
        /// <summary>
        /// The data to decrypt
        /// </summary>
        public System.ArraySegment<byte> Data { get; set; }

        /// <summary>
        /// The size in bytes of OutBuffer
        /// </summary>
        public uint OutBufferSizeBytes { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct UnprotectMessageOptionsInternal : ISettable<UnprotectMessageOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private uint m_DataLengthBytes;
        private System.IntPtr m_Data;
        private uint m_OutBufferSizeBytes;

        public System.ArraySegment<byte> Data
        {
            set
            {
                Helper.Set(value, ref m_Data, out m_DataLengthBytes);
            }
        }

        public uint OutBufferSizeBytes
        {
            set
            {
                m_OutBufferSizeBytes = value;
            }
        }

        public void Set(ref UnprotectMessageOptions other)
        {
            m_ApiVersion = AntiCheatClientInterface.UnprotectmessageApiLatest;
            Data = other.Data;
            OutBufferSizeBytes = other.OutBufferSizeBytes;
        }

        public void Set(ref UnprotectMessageOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = AntiCheatClientInterface.UnprotectmessageApiLatest;
                Data = other.Value.Data;
                OutBufferSizeBytes = other.Value.OutBufferSizeBytes;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_Data);
        }
    }
}