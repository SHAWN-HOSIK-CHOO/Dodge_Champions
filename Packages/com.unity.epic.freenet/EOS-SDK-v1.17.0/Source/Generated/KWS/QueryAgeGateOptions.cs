// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.KWS
{
    /// <summary>
    /// Input parameters for the <see cref="KWSInterface.QueryAgeGate" /> function.
    /// </summary>
    public struct QueryAgeGateOptions
    {
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct QueryAgeGateOptionsInternal : ISettable<QueryAgeGateOptions>, System.IDisposable
    {
        private int m_ApiVersion;

        public void Set(ref QueryAgeGateOptions other)
        {
            m_ApiVersion = KWSInterface.QueryagegateApiLatest;
        }

        public void Set(ref QueryAgeGateOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = KWSInterface.QueryagegateApiLatest;
            }
        }

        public void Dispose()
        {
        }
    }
}