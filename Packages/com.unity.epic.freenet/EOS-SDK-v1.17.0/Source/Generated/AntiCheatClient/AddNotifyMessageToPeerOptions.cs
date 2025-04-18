// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.AntiCheatClient
{
    public struct AddNotifyMessageToPeerOptions
    {
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct AddNotifyMessageToPeerOptionsInternal : ISettable<AddNotifyMessageToPeerOptions>, System.IDisposable
    {
        private int m_ApiVersion;

        public void Set(ref AddNotifyMessageToPeerOptions other)
        {
            m_ApiVersion = AntiCheatClientInterface.AddnotifymessagetopeerApiLatest;
        }

        public void Set(ref AddNotifyMessageToPeerOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = AntiCheatClientInterface.AddnotifymessagetopeerApiLatest;
            }
        }

        public void Dispose()
        {
        }
    }
}