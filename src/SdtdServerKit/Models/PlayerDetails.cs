namespace SdtdServerKit.Models
{
    /// <summary>
    /// Represents the details of a player.
    /// </summary>
    public class PlayerDetails
    {
        private readonly IManagedPlayer _player;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDetails"/> class.
        /// </summary>
        /// <param name="player">The player object.</param>
        public PlayerDetails(IManagedPlayer player)
        {
            _player = player;
        }

        /// <summary>
        /// Gets the player's admin status.
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                var users = GameManager.Instance.adminTools.Users;
                if (_player.ClientInfo != null)
                {
                    return users.GetUserPermissionLevel(this._player.ClientInfo) == 0;
                }
                else
                {
                    return users.GetUserPermissionLevel(this._player.PersistentPlayerData.PrimaryId) == 0;
                }
            }
        }

        /// <summary>
        /// Gets the position of the player.
        /// </summary>
        public Position Position => _player.PlayerDataFile.ecd.pos.ToPosition();

        /// <summary>
        /// Gets the last spawn position of the player.
        /// </summary>
        public SpawnPosition LastSpawnPosition => _player.PlayerDataFile.lastSpawnPosition.ToModel();

        /// <summary>
        /// Gets the last login time of the player.
        /// </summary>
        public DateTime LastLogin => _player.PersistentPlayerData.LastLogin;

        /// <summary>
        /// Gets the number of kills by the player.
        /// </summary>
        public int PlayerKills => _player.PlayerDataFile.playerKills;

        /// <summary>
        /// Gets the number of zombie kills by the player.
        /// </summary>
        public int ZombieKills => _player.PlayerDataFile.zombieKills;

        /// <summary>
        /// Gets the number of deaths by the player.
        /// </summary>
        public int Deaths => _player.PlayerDataFile.deaths;

        /// <summary>
        /// Gets the score of the player.
        /// </summary>
        public int Score => _player.PlayerDataFile.score;

        /// <summary>
        /// Gets the player stats.
        /// </summary>
        public PlayerStats Stats
        {
            get
            {
                var stats = _player.PlayerDataFile.ecd.stats;
                return new PlayerStats()
                {
                    Health = stats.Health.Value,
                    Stamina = stats.Stamina.Value,
                    CoreTemp = stats.CoreTemp.Value,
                    Food = stats.Food.Value,
                    Water = stats.Water.Value
                };
            }
        }

        /// <summary>
        /// Gets the player progression.
        /// </summary>
        public PlayerProgression Progression
        {
            get
            {
                var entityPlayer = _player.EntityPlayer;
                if (entityPlayer != null)
                {
                    return new PlayerProgression()
                    {
                        Level = entityPlayer.Progression.Level,
                        ExpToNextLevel = entityPlayer.Progression.ExpToNextLevel,
                        SkillPoints = entityPlayer.Progression.SkillPoints
                    };
                }
                else
                {
                    var progression = new PlayerProgression();
                    var stream = _player.PlayerDataFile.progressionData;
                    if (stream.Length > 0L)
                    {
                        using var binaryReader = MemoryPools.poolBinaryReader.AllocSync(false);
                        stream.Position = 0L;
                        binaryReader.SetBaseStream(stream);

                        byte b = binaryReader.ReadByte();
                        progression.Level = binaryReader.ReadUInt16();
                        progression.ExpToNextLevel = binaryReader.ReadInt32();
                        progression.SkillPoints = binaryReader.ReadUInt16();
                    }

                    return progression;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether land protection is active for the player.
        /// </summary>
        public bool LandProtectionActive => GameManager.Instance.World.IsLandProtectionValidForPlayer(_player.PersistentPlayerData);

        /// <summary>
        /// Gets the distance walked by the player.
        /// </summary>
        public float DistanceWalked => _player.PlayerDataFile.distanceWalked;

        /// <summary>
        /// Gets the total number of items crafted by the player.
        /// </summary>
        public uint TotalItemsCrafted => _player.PlayerDataFile.totalItemsCrafted;

        /// <summary>
        /// Gets the longest life of the player.
        /// </summary>
        public float LongestLife => _player.PlayerDataFile.longestLife;

        /// <summary>
        /// Gets the current life of the player.
        /// </summary>
        public float CurrentLife => _player.PlayerDataFile.currentLife;

        /// <summary>
        /// Gets the total time played by the player in minutes.
        /// </summary>
        public float TotalTimePlayed => _player.PlayerDataFile.totalTimePlayed;

        /// <summary>
        /// Gets the rented VM position of the player.
        /// </summary>
        public Position RentedVMPosition => _player.PlayerDataFile.rentedVMPosition.ToPosition();

        /// <summary>
        /// Gets the rental end time of the player.
        /// </summary>
        public ulong RentalEndTime => _player.PlayerDataFile.rentalEndTime;

        /// <summary>
        /// Gets the rental end day of the player.
        /// </summary>
        public int RentalEndDay => _player.PlayerDataFile.rentalEndDay;

        /// <summary>
        /// Gets the spawn points of the player.
        /// </summary>
        public IEnumerable<Position> SpawnPoints => _player.PlayerDataFile.spawnPoints.ToPositions();

        /// <summary>
        /// Gets the list of already crafted items by the player.
        /// </summary>
        public IEnumerable<string> AlreadyCraftedList => _player.PlayerDataFile.alreadyCraftedList;

        /// <summary>
        /// Gets the list of unlocked recipes by the player.
        /// </summary>
        public IEnumerable<string> UnlockedRecipeList => _player.PlayerDataFile.unlockedRecipeList;

        /// <summary>
        /// Gets the list of favorite recipes by the player.
        /// </summary>
        public IEnumerable<string> FavoriteRecipeList => _player.PlayerDataFile.favoriteRecipeList;

        /// <summary>
        /// Gets the list of owned entities by the player.
        /// </summary>
        public IEnumerable<OwnedEntity> OwnedEntities => _player.PlayerDataFile.ownedEntities.ToModels();

        /// <summary>
        /// Gets or sets the count of points.
        /// </summary>
        public int PointsCount { get; set; }

    }
}