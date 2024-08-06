namespace SdtdServerKit.Managers
{
    /// <summary>
    /// Represents a collection of online players.
    /// </summary>
    public static class OnlinePlayerManager
    {
        private static readonly ThreadSafeReadDictionary<int, OnlinePlayer> _entityIdMap;
        private static readonly ThreadSafeReadDictionary<string, OnlinePlayer> _playerIdMap;

        static OnlinePlayerManager()
        {
            _entityIdMap = new ThreadSafeReadDictionary<int, OnlinePlayer>();
            _playerIdMap = new ThreadSafeReadDictionary<string, OnlinePlayer>();
        }

        /// <summary>
        /// Adds a new online player to the collection.
        /// </summary>
        /// <param name="clientInfo">The client information of the player.</param>
        /// <returns>The added online player.</returns>
        internal static OnlinePlayer Add(ClientInfo clientInfo)
        {
            var onlinePlayer = new OnlinePlayer(clientInfo);
            _entityIdMap.Add(onlinePlayer.EntityId, onlinePlayer);
            _playerIdMap.Add(onlinePlayer.PlayerId, onlinePlayer);
            return onlinePlayer;
        }

        /// <summary>
        /// Removes an online player from the collection.
        /// </summary>
        /// <param name="clientInfo">The client information of the player.</param>
        internal static void Remove(ClientInfo clientInfo)
        {
            _entityIdMap.Remove(clientInfo.entityId);
            _playerIdMap.Remove(clientInfo.InternalId.CombinedString);
        }

        /// <summary>
        /// Gets an online player by their entity ID.
        /// </summary>
        /// <param name="entityId">The entity ID of the player.</param>
        /// <returns>The online player with the specified entity ID, or throw exception if not found.</returns>
        public static OnlinePlayer GetByEntityId(int entityId)
        {
            return _entityIdMap.Read(entityId);
        }

        /// <summary>
        /// Gets an online player by their player ID.
        /// </summary>
        /// <param name="playerId">The player ID of the player.</param>
        /// <returns>The online player with the specified player ID, or throw exception if not found.</returns>
        public static OnlinePlayer GetByPlayerId(string playerId)
        {
            return _playerIdMap.Read(playerId);
        }

        /// <summary>
        /// Tries to get an online player by their entity ID.
        /// </summary>
        /// <param name="entityId">The entity ID of the player.</param>
        /// <param name="onlinePlayer">The online player with the specified entity ID, if found; otherwise, null.</param>
        /// <returns>true if the online player with the specified entity ID is found; otherwise, false.</returns>
        public static bool TryGetByEntityId(int entityId, out OnlinePlayer? onlinePlayer)
        {
            return _entityIdMap.TryRead(entityId, out onlinePlayer);
        }

        /// <summary>
        /// Tries to get an online player by their player ID.
        /// </summary>
        /// <param name="playerId">The player ID of the player.</param>
        /// <param name="onlinePlayer">The online player with the specified player ID, if found; otherwise, null.</param>
        /// <returns>true if the online player with the specified player ID is found; otherwise, false.</returns>
        public static bool TryGetByPlayerId(string playerId, out OnlinePlayer? onlinePlayer)
        {
            return _playerIdMap.TryRead(playerId, out onlinePlayer);
        }

        /// <summary>
        /// Gets all online players.
        /// </summary>
        /// <returns>An enumerable collection of online players.</returns>
        public static IEnumerable<OnlinePlayer> GetAll ()
        {
            return _entityIdMap.GetValues();
        }
    }
}
