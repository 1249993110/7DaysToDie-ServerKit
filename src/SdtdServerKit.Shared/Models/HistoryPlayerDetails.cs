namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// History Player Details
    /// </summary>
    public class HistoryPlayerDetails : HistoryPlayer, IPlayerDetails
    {
        public HistoryPlayerDetails() { }

        public HistoryPlayerDetails(HistoryPlayer historyPlayer)
        {
            PlatformId = historyPlayer.PlatformId;
            CrossplatformId = historyPlayer.CrossplatformId;
            PlayerName = historyPlayer.PlayerName;
            Position = historyPlayer.Position;
            LastLogin = historyPlayer.LastLogin;
        }

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
        public IEnumerable<Position> SpawnPoints { get; set; } = null!;

        /// <summary>
        /// Already Crafted List
        /// </summary>
        public IEnumerable<string> AlreadyCraftedList { get; set; } = null!;

        /// <summary>
        /// Last Spawn Position
        /// </summary>
        public SpawnPosition LastSpawnPosition { get; set; }

        /// <summary>
        /// Unlocked Recipe List
        /// </summary>
        public IEnumerable<string> UnlockedRecipeList { get; set; } = null!;

        /// <summary>
        /// Favorite Recipe List
        /// </summary>
        public IEnumerable<string> FavoriteRecipeList { get; set; } = null!;

        /// <summary>
        /// Player OwnedE ntities
        /// </summary>
        public IEnumerable<OwnedEntity> OwnedEntities { get; set; } = null!;

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
