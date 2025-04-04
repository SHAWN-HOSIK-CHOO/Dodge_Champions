// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyDetails.CopyInfo" /> function.
    /// </summary>
    public struct LobbyDetailsCopyInfoOptions
    {
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LobbyDetailsCopyInfoOptionsInternal : ISettable<LobbyDetailsCopyInfoOptions>, System.IDisposable
    {
        private int m_ApiVersion;

        public void Set(ref LobbyDetailsCopyInfoOptions other)
        {
            m_ApiVersion = LobbyDetails.LobbydetailsCopyinfoApiLatest;
        }

        public void Set(ref LobbyDetailsCopyInfoOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyDetails.LobbydetailsCopyinfoApiLatest;
            }
        }

        public void Dispose()
        {
        }
    }
}