namespace SdtdServerKit.Extensions
{
    public static class ClientInfoExtension
    {
        public static OnlinePlayer ToOnlinePlayer(this ClientInfo clientInfo)
        {
            return new OnlinePlayer()
            {
                EntityId = clientInfo.entityId,
                PlatformId = clientInfo.PlatformId.CombinedString,
                CrossplatformId = clientInfo.CrossplatformId.CombinedString,
                PlayerName = clientInfo.playerName,
                Ip = clientInfo.ip,
                Ping = clientInfo.ping,
            };
        }

        public static OnlinePlayerDetails ToOnlinePlayerDetails(this ClientInfo clientInfo)
        {
            var latestPlayerData = clientInfo.latestPlayerData;
            var persistentPlayerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(clientInfo.InternalId);
            var onlinePlayerDetails = latestPlayerData.ToPlayerDetails<OnlinePlayerDetails>(persistentPlayerData);
            onlinePlayerDetails.Ip = clientInfo.ip;
            onlinePlayerDetails.Ping = clientInfo.ping;
            return onlinePlayerDetails;
        }

        public static SpawnedPlayer ToSpawnedPlayer(this ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
        {
            return new SpawnedPlayer()
            {
                EntityId = clientInfo.entityId,
                PlatformId = clientInfo.PlatformId.CombinedString,
                CrossplatformId = clientInfo.CrossplatformId.CombinedString,
                PlayerName = clientInfo.playerName,
                Ip = clientInfo.ip,
                Ping = clientInfo.ping,
                RespawnType = (Shared.Models.RespawnType)respawnType,
                Position = position.ToPosition(),
            };
        }
    }
}