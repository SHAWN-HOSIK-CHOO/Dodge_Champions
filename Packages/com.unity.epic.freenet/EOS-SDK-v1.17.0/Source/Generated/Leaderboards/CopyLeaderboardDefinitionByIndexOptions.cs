// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Leaderboards
{
    /// <summary>
    /// Input parameters for the <see cref="LeaderboardsInterface.CopyLeaderboardDefinitionByIndex" /> function.
    /// </summary>
    public struct CopyLeaderboardDefinitionByIndexOptions
    {
        /// <summary>
        /// Index of the leaderboard definition to retrieve from the cache
        /// </summary>
        public uint LeaderboardIndex { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyLeaderboardDefinitionByIndexOptionsInternal : ISettable<CopyLeaderboardDefinitionByIndexOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private uint m_LeaderboardIndex;

        public uint LeaderboardIndex
        {
            set
            {
                m_LeaderboardIndex = value;
            }
        }

        public void Set(ref CopyLeaderboardDefinitionByIndexOptions other)
        {
            m_ApiVersion = LeaderboardsInterface.CopyleaderboarddefinitionbyindexApiLatest;
            LeaderboardIndex = other.LeaderboardIndex;
        }

        public void Set(ref CopyLeaderboardDefinitionByIndexOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LeaderboardsInterface.CopyleaderboarddefinitionbyindexApiLatest;
                LeaderboardIndex = other.Value.LeaderboardIndex;
            }
        }

        public void Dispose()
        {
        }
    }
}