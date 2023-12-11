﻿namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// Interface Player Details
    /// </summary>
    public interface IPlayerDetails
    {
        /// <summary>
        /// 平台Id
        /// </summary>
        public string PlatformId { get; set; }

        /// <summary>
        /// 跨平台Id
        /// </summary>
        public string CrossplatformId { get; set; }

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 最后登录
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <summary>
        /// Entity Id
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Player Kills
        /// </summary>
        public int PlayerKills { get; set; }

        /// <summary>
        /// Zombie Kills
        /// </summary>
        public int ZombieKills { get; set; }

        /// <summary>
        /// Deaths
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Health
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// Stamina
        /// </summary>
        public float Stamina { get; set; }

        /// <summary>
        /// CoreTemp
        /// </summary>
        public float CoreTemp { get; set; }

        /// <summary>
        /// Food
        /// </summary>
        public float Food { get; set; }

        /// <summary>
        /// Water
        /// </summary>
        public float Water { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Experience To Next Level
        /// </summary>
        public int ExpToNextLevel { get; set; }

        /// <summary>
        /// Skill Points
        /// </summary>
        public int SkillPoints { get; set; }

        /// <summary>
        /// Land Protection Active
        /// </summary>
        public bool LandProtectionActive { get; set; }

        /// <summary>
        /// Land Protection Multiplier
        /// </summary>
        public float LandProtectionMultiplier { get; set; }

        /// <summary>
        /// Spawn Points
        /// </summary>
        public IEnumerable<Position> SpawnPoints { get; set; }

        /// <summary>
        /// Already Crafted List
        /// </summary>
        public IEnumerable<string> AlreadyCraftedList { get; set; }

        /// <summary>
        /// Last Spawn Position
        /// </summary>
        public SpawnPosition LastSpawnPosition { get; set; }

        /// <summary>
        /// Unlocked Recipe List
        /// </summary>
        public IEnumerable<string> UnlockedRecipeList { get; set; }

        /// <summary>
        /// Favorite Recipe List
        /// </summary>
        public IEnumerable<string> FavoriteRecipeList { get; set; }

        /// <summary>
        /// Player OwnedE ntities
        /// </summary>
        public IEnumerable<OwnedEntity> OwnedEntities { get; set; }

        /// <summary>
        /// Distance Walked
        /// </summary>
        public float DistanceWalked { get; set; }

        /// <summary>
        /// Total Items Crafted
        /// </summary>
        public uint TotalItemsCrafted { get; set; }

        /// <summary>
        /// Longest Life
        /// </summary>
        public float LongestLife { get; set; }

        /// <summary>
        /// Current Life
        /// </summary>
        public float CurrentLife { get; set; }

        /// <summary>
        /// Total Time Played, Unit: minutes
        /// </summary>
        public float TotalTimePlayed { get; set; }

        /// <summary>
        /// Game Stage Born At World Time
        /// </summary>
        public ulong GameStageBornAtWorldTime { get; set; }

        /// <summary>
        /// Rented VM Position
        /// </summary>
        public Position RentedVMPosition { get; set; }

        /// <summary>
        /// Rental End Time
        /// </summary>
        public ulong RentalEndTime { get; set; }

        /// <summary>
        /// Rental End Day
        /// </summary>
        public int RentalEndDay { get; set; }
    }
}
