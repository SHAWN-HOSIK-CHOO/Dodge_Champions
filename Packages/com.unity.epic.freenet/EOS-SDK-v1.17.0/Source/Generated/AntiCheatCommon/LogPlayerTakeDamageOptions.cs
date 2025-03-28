// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.AntiCheatCommon
{
    public struct LogPlayerTakeDamageOptions
    {
        /// <summary>
        /// Locally unique value used in RegisterClient/RegisterPeer
        /// </summary>
        public System.IntPtr VictimPlayerHandle { get; set; }

        /// <summary>
        /// Victim player character's world position as a 3D vector. This should be the center of the character.
        /// </summary>
        public Vec3f? VictimPlayerPosition { get; set; }

        /// <summary>
        /// Victim player camera's world rotation as a quaternion.
        /// </summary>
        public Quat? VictimPlayerViewRotation { get; set; }

        /// <summary>
        /// Locally unique value used in RegisterClient/RegisterPeer if applicable, otherwise 0.
        /// </summary>
        public System.IntPtr AttackerPlayerHandle { get; set; }

        /// <summary>
        /// Attacker player character's world position as a 3D vector if applicable, otherwise <see langword="null" />.
        /// </summary>
        public Vec3f? AttackerPlayerPosition { get; set; }

        /// <summary>
        /// Attacker player camera's world rotation as a quaternion if applicable, otherwise <see langword="null" />.
        /// </summary>
        public Quat? AttackerPlayerViewRotation { get; set; }

        /// <summary>
        /// True if the damage was applied instantly at the time of attack from the game
        /// simulation's perspective, otherwise false (simulated ballistics, arrow, etc).
        /// </summary>
        public bool IsHitscanAttack { get; set; }

        /// <summary>
        /// True if there is a visible line of sight between the attacker and the victim at the time
        /// that damage is being applied, false if there is an obstacle like a wall or terrain in
        /// the way. For some situations like melee or hitscan weapons this is trivially
        /// true, for others like projectiles with simulated physics it may not be e.g. a player
        /// could fire a slow moving projectile and then move behind cover before it strikes.
        /// 
        /// This can be an estimate, or can simply be always set to true if it is not feasible
        /// to compute in your game.
        /// </summary>
        public bool HasLineOfSight { get; set; }

        /// <summary>
        /// True if this was a critical hit that causes extra damage (e.g. headshot)
        /// </summary>
        public bool IsCriticalHit { get; set; }

        /// <summary>
        /// Deprecated - use DamagePosition instead
        /// </summary>
        internal uint HitBoneId_DEPRECATED { get; set; }

        /// <summary>
        /// Number of health points that the victim lost due to this damage event
        /// </summary>
        public float DamageTaken { get; set; }

        /// <summary>
        /// Number of health points that the victim has remaining after this damage event
        /// </summary>
        public float HealthRemaining { get; set; }

        /// <summary>
        /// Source of the damage event
        /// </summary>
        public AntiCheatCommonPlayerTakeDamageSource DamageSource { get; set; }

        /// <summary>
        /// Type of the damage being applied
        /// </summary>
        public AntiCheatCommonPlayerTakeDamageType DamageType { get; set; }

        /// <summary>
        /// Result of the damage for the victim, if any
        /// </summary>
        public AntiCheatCommonPlayerTakeDamageResult DamageResult { get; set; }

        /// <summary>
        /// PlayerUseWeaponData associated with this damage event if available, otherwise <see langword="null" />
        /// </summary>
        public LogPlayerUseWeaponData? PlayerUseWeaponData { get; set; }

        /// <summary>
        /// Time in milliseconds since the associated PlayerUseWeaponData event occurred if available, otherwise 0
        /// </summary>
        public uint TimeSincePlayerUseWeaponMs { get; set; }

        /// <summary>
        /// World position where damage hit the victim as a 3D vector if available, otherwise <see langword="null" />
        /// </summary>
        public Vec3f? DamagePosition { get; set; }

        /// <summary>
        /// Attacker player camera's world position as a 3D vector if applicable, otherwise <see langword="null" />
        /// </summary>
        public Vec3f? AttackerPlayerViewPosition { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LogPlayerTakeDamageOptionsInternal : ISettable<LogPlayerTakeDamageOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_VictimPlayerHandle;
        private System.IntPtr m_VictimPlayerPosition;
        private System.IntPtr m_VictimPlayerViewRotation;
        private System.IntPtr m_AttackerPlayerHandle;
        private System.IntPtr m_AttackerPlayerPosition;
        private System.IntPtr m_AttackerPlayerViewRotation;
        private int m_IsHitscanAttack;
        private int m_HasLineOfSight;
        private int m_IsCriticalHit;
        private uint m_HitBoneId_DEPRECATED;
        private float m_DamageTaken;
        private float m_HealthRemaining;
        private AntiCheatCommonPlayerTakeDamageSource m_DamageSource;
        private AntiCheatCommonPlayerTakeDamageType m_DamageType;
        private AntiCheatCommonPlayerTakeDamageResult m_DamageResult;
        private System.IntPtr m_PlayerUseWeaponData;
        private uint m_TimeSincePlayerUseWeaponMs;
        private System.IntPtr m_DamagePosition;
        private System.IntPtr m_AttackerPlayerViewPosition;

        public System.IntPtr VictimPlayerHandle
        {
            set
            {
                m_VictimPlayerHandle = value;
            }
        }

        public Vec3f? VictimPlayerPosition
        {
            set
            {
                Helper.Set<Vec3f, Vec3fInternal>(ref value, ref m_VictimPlayerPosition);
            }
        }

        public Quat? VictimPlayerViewRotation
        {
            set
            {
                Helper.Set<Quat, QuatInternal>(ref value, ref m_VictimPlayerViewRotation);
            }
        }

        public System.IntPtr AttackerPlayerHandle
        {
            set
            {
                m_AttackerPlayerHandle = value;
            }
        }

        public Vec3f? AttackerPlayerPosition
        {
            set
            {
                Helper.Set<Vec3f, Vec3fInternal>(ref value, ref m_AttackerPlayerPosition);
            }
        }

        public Quat? AttackerPlayerViewRotation
        {
            set
            {
                Helper.Set<Quat, QuatInternal>(ref value, ref m_AttackerPlayerViewRotation);
            }
        }

        public bool IsHitscanAttack
        {
            set
            {
                Helper.Set(value, ref m_IsHitscanAttack);
            }
        }

        public bool HasLineOfSight
        {
            set
            {
                Helper.Set(value, ref m_HasLineOfSight);
            }
        }

        public bool IsCriticalHit
        {
            set
            {
                Helper.Set(value, ref m_IsCriticalHit);
            }
        }

        public uint HitBoneId_DEPRECATED
        {
            set
            {
                m_HitBoneId_DEPRECATED = value;
            }
        }

        public float DamageTaken
        {
            set
            {
                m_DamageTaken = value;
            }
        }

        public float HealthRemaining
        {
            set
            {
                m_HealthRemaining = value;
            }
        }

        public AntiCheatCommonPlayerTakeDamageSource DamageSource
        {
            set
            {
                m_DamageSource = value;
            }
        }

        public AntiCheatCommonPlayerTakeDamageType DamageType
        {
            set
            {
                m_DamageType = value;
            }
        }

        public AntiCheatCommonPlayerTakeDamageResult DamageResult
        {
            set
            {
                m_DamageResult = value;
            }
        }

        public LogPlayerUseWeaponData? PlayerUseWeaponData
        {
            set
            {
                Helper.Set<LogPlayerUseWeaponData, LogPlayerUseWeaponDataInternal>(ref value, ref m_PlayerUseWeaponData);
            }
        }

        public uint TimeSincePlayerUseWeaponMs
        {
            set
            {
                m_TimeSincePlayerUseWeaponMs = value;
            }
        }

        public Vec3f? DamagePosition
        {
            set
            {
                Helper.Set<Vec3f, Vec3fInternal>(ref value, ref m_DamagePosition);
            }
        }

        public Vec3f? AttackerPlayerViewPosition
        {
            set
            {
                Helper.Set<Vec3f, Vec3fInternal>(ref value, ref m_AttackerPlayerViewPosition);
            }
        }

        public void Set(ref LogPlayerTakeDamageOptions other)
        {
            m_ApiVersion = AntiCheatCommonInterface.LogplayertakedamageApiLatest;
            VictimPlayerHandle = other.VictimPlayerHandle;
            VictimPlayerPosition = other.VictimPlayerPosition;
            VictimPlayerViewRotation = other.VictimPlayerViewRotation;
            AttackerPlayerHandle = other.AttackerPlayerHandle;
            AttackerPlayerPosition = other.AttackerPlayerPosition;
            AttackerPlayerViewRotation = other.AttackerPlayerViewRotation;
            IsHitscanAttack = other.IsHitscanAttack;
            HasLineOfSight = other.HasLineOfSight;
            IsCriticalHit = other.IsCriticalHit;
            HitBoneId_DEPRECATED = other.HitBoneId_DEPRECATED;
            DamageTaken = other.DamageTaken;
            HealthRemaining = other.HealthRemaining;
            DamageSource = other.DamageSource;
            DamageType = other.DamageType;
            DamageResult = other.DamageResult;
            PlayerUseWeaponData = other.PlayerUseWeaponData;
            TimeSincePlayerUseWeaponMs = other.TimeSincePlayerUseWeaponMs;
            DamagePosition = other.DamagePosition;
            AttackerPlayerViewPosition = other.AttackerPlayerViewPosition;
        }

        public void Set(ref LogPlayerTakeDamageOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = AntiCheatCommonInterface.LogplayertakedamageApiLatest;
                VictimPlayerHandle = other.Value.VictimPlayerHandle;
                VictimPlayerPosition = other.Value.VictimPlayerPosition;
                VictimPlayerViewRotation = other.Value.VictimPlayerViewRotation;
                AttackerPlayerHandle = other.Value.AttackerPlayerHandle;
                AttackerPlayerPosition = other.Value.AttackerPlayerPosition;
                AttackerPlayerViewRotation = other.Value.AttackerPlayerViewRotation;
                IsHitscanAttack = other.Value.IsHitscanAttack;
                HasLineOfSight = other.Value.HasLineOfSight;
                IsCriticalHit = other.Value.IsCriticalHit;
                HitBoneId_DEPRECATED = other.Value.HitBoneId_DEPRECATED;
                DamageTaken = other.Value.DamageTaken;
                HealthRemaining = other.Value.HealthRemaining;
                DamageSource = other.Value.DamageSource;
                DamageType = other.Value.DamageType;
                DamageResult = other.Value.DamageResult;
                PlayerUseWeaponData = other.Value.PlayerUseWeaponData;
                TimeSincePlayerUseWeaponMs = other.Value.TimeSincePlayerUseWeaponMs;
                DamagePosition = other.Value.DamagePosition;
                AttackerPlayerViewPosition = other.Value.AttackerPlayerViewPosition;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_VictimPlayerHandle);
            Helper.Dispose(ref m_VictimPlayerPosition);
            Helper.Dispose(ref m_VictimPlayerViewRotation);
            Helper.Dispose(ref m_AttackerPlayerHandle);
            Helper.Dispose(ref m_AttackerPlayerPosition);
            Helper.Dispose(ref m_AttackerPlayerViewRotation);
            Helper.Dispose(ref m_PlayerUseWeaponData);
            Helper.Dispose(ref m_DamagePosition);
            Helper.Dispose(ref m_AttackerPlayerViewPosition);
        }
    }
}