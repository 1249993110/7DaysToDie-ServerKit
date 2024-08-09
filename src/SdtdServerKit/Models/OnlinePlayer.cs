namespace SdtdServerKit.Models
{
    /// <summary>
    /// Online player in the game.
    /// </summary>
    public class OnlinePlayer : IManagedPlayer
    {
        private readonly ClientInfo _clientInfo;
        private readonly PersistentPlayerData _persistentPlayerData;
        private readonly EntityPlayer _entityPlayer;
        private readonly PlayerDetails _playerDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlinePlayer"/> class.
        /// </summary>
        /// <param name="clientInfo">The client information.</param>
        public OnlinePlayer(ClientInfo clientInfo)
        {
            _clientInfo = clientInfo;
            _persistentPlayerData = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(clientInfo.entityId);
            _entityPlayer = GameManager.Instance.World.Players.dict[clientInfo.entityId];
            _playerDetails = new PlayerDetails(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public PersistentPlayerData PersistentPlayerData => _persistentPlayerData;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public PlayerDataFile PlayerDataFile => _clientInfo.latestPlayerData;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ClientInfo ClientInfo => _clientInfo;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public EntityPlayer EntityPlayer => _entityPlayer;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlayerId => _clientInfo.InternalId.CombinedString;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlayerName => _clientInfo.playerName;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int EntityId => _clientInfo.entityId;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlatformId => _clientInfo.PlatformId.CombinedString;

        /// <summary>
        /// Gets the player details.
        /// </summary>
        public PlayerDetails PlayerDetails => _playerDetails;

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        public string Ip => _clientInfo.ip;

        /// <summary>
        /// Gets the ping value.
        /// </summary>
        public int Ping => _clientInfo.ping;

        /// <summary>
        /// Gets the game stage of the player.
        /// </summary>
        public int GameStage => _entityPlayer.gameStage;

    }
}