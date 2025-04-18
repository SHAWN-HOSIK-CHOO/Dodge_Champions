// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyInterface.CopyLobbyDetailsHandleByUiEventId" /> function.
    /// </summary>
    public struct CopyLobbyDetailsHandleByUiEventIdOptions
    {
        /// <summary>
        /// UI Event associated with the lobby
        /// </summary>
        public ulong UiEventId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyLobbyDetailsHandleByUiEventIdOptionsInternal : ISettable<CopyLobbyDetailsHandleByUiEventIdOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private ulong m_UiEventId;

        public ulong UiEventId
        {
            set
            {
                m_UiEventId = value;
            }
        }

        public void Set(ref CopyLobbyDetailsHandleByUiEventIdOptions other)
        {
            m_ApiVersion = LobbyInterface.CopylobbydetailshandlebyuieventidApiLatest;
            UiEventId = other.UiEventId;
        }

        public void Set(ref CopyLobbyDetailsHandleByUiEventIdOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyInterface.CopylobbydetailshandlebyuieventidApiLatest;
                UiEventId = other.Value.UiEventId;
            }
        }

        public void Dispose()
        {
        }
    }
}