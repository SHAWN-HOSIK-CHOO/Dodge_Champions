// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbySearch.GetSearchResultCount" /> function.
    /// </summary>
    public struct LobbySearchGetSearchResultCountOptions
    {
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LobbySearchGetSearchResultCountOptionsInternal : ISettable<LobbySearchGetSearchResultCountOptions>, System.IDisposable
    {
        private int m_ApiVersion;

        public void Set(ref LobbySearchGetSearchResultCountOptions other)
        {
            m_ApiVersion = LobbySearch.LobbysearchGetsearchresultcountApiLatest;
        }

        public void Set(ref LobbySearchGetSearchResultCountOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbySearch.LobbysearchGetsearchresultcountApiLatest;
            }
        }

        public void Dispose()
        {
        }
    }
}