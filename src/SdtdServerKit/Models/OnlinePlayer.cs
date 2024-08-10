namespace SdtdServerKit.Models
{
    /// <summary>
    /// Represents an online player for response model.
    /// </summary>
    public class OnlinePlayer : PlayerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnlinePlayer"/> class.
        /// </summary>
        public OnlinePlayer(IManagedPlayer managedPlayer) : base(managedPlayer)
        {
            var clientInfo = managedPlayer.ClientInfo!;
            var entityPlayer = managedPlayer.EntityPlayer!;
            Ip = clientInfo.ip;
            Ping = clientInfo.ping;
            GameStage = entityPlayer.gameStage;
            PlayerDetails = new PlayerDetails(managedPlayer.PlayerDataFile, managedPlayer.PersistentPlayerData, entityPlayer);
        }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets the ping value.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Gets the game stage of the player.
        /// </summary>
        public int GameStage { get; set; }

        /// <summary>
        /// Gets the player details.
        /// </summary>
        public PlayerDetails PlayerDetails { get; set; }
    }
}
