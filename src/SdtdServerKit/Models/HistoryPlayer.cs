using SdtdServerKit.Managers;

namespace SdtdServerKit.Models
{
    /// <summary>
    /// History Player
    /// </summary>
    public class HistoryPlayer : IManagedPlayer
    {
        private readonly PersistentPlayerData _persistentPlayerData;
        private readonly PlayerDetails _playerDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryPlayer"/> class.
        /// </summary>
        /// <param name="persistentPlayerData">The client information.</param>
        public HistoryPlayer(PersistentPlayerData persistentPlayerData)
        {
            _persistentPlayerData = persistentPlayerData;
            _playerDetails = new PlayerDetails(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public PersistentPlayerData PersistentPlayerData => _persistentPlayerData;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public PlayerDataFile PlayerDataFile
        {
            get
            {
                if (OnlinePlayerManager.TryGetByEntityId(EntityId, out var onlinePlayer))
                {
                    return onlinePlayer!.PlayerDataFile;
                }
                else
                {
                    var playerDataFile = new PlayerDataFile();
                    playerDataFile.Load(GameIO.GetPlayerDataDir(), PlayerId);
                    return playerDataFile;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ClientInfo? ClientInfo
        {
            get
            {
                if (OnlinePlayerManager.TryGetByEntityId(EntityId, out var onlinePlayer))
                {
                    return onlinePlayer!.ClientInfo;
                }

                return null;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public EntityPlayer? EntityPlayer
        {
            get
            {
                if (IsOffline == false && GameManager.Instance.World.Players.dict.TryGetValue(EntityId, out EntityPlayer entityPlayer))
                {
                    return entityPlayer;
                }

                return null;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlayerId => _persistentPlayerData.PrimaryId.CombinedString;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlayerName => _persistentPlayerData.PlayerName.DisplayName;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int EntityId => _persistentPlayerData.EntityId;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlatformId => _persistentPlayerData.nativeId.CombinedString;

        /// <summary>
        /// Gets the player details.
        /// </summary>
        public PlayerDetails PlayerDetails => _playerDetails;

        /// <summary>
        /// Gets a value indicating whether the player is offline.
        /// </summary>
        public bool IsOffline => ClientInfo == null;
    }
}
