// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Achievements
{
    /// <summary>
    /// Input parameters for the <see cref="AchievementsInterface.CopyAchievementDefinitionV2ByAchievementId" /> function.
    /// </summary>
    public struct CopyAchievementDefinitionV2ByAchievementIdOptions
    {
        /// <summary>
        /// Achievement ID to look for when copying the definition from the cache.
        /// </summary>
        public Utf8String AchievementId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyAchievementDefinitionV2ByAchievementIdOptionsInternal : ISettable<CopyAchievementDefinitionV2ByAchievementIdOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_AchievementId;

        public Utf8String AchievementId
        {
            set
            {
                Helper.Set(value, ref m_AchievementId);
            }
        }

        public void Set(ref CopyAchievementDefinitionV2ByAchievementIdOptions other)
        {
            m_ApiVersion = AchievementsInterface.Copyachievementdefinitionv2ByachievementidApiLatest;
            AchievementId = other.AchievementId;
        }

        public void Set(ref CopyAchievementDefinitionV2ByAchievementIdOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = AchievementsInterface.Copyachievementdefinitionv2ByachievementidApiLatest;
                AchievementId = other.Value.AchievementId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_AchievementId);
        }
    }
}