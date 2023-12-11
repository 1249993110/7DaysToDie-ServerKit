namespace SdtdServerKit.Extensions
{
    public static class PersistentPlayerDataExtension
    {
        public static HistoryPlayer ToHistoryPlayer(this PersistentPlayerData persistentPlayerData)
        {
            return new HistoryPlayer()
            {
                PlatformId = persistentPlayerData.PlatformUserIdentifier.CombinedString,
                CrossplatformId = persistentPlayerData.UserIdentifier.CombinedString,
                PlayerName = persistentPlayerData.PlayerName,
                Position = persistentPlayerData.Position.ToPosition(),
                LastLogin = persistentPlayerData.LastLogin,
            };
        }

        public static HistoryPlayerDetails ToHistoryPlayerDetails(this PersistentPlayerData persistentPlayerData)
        {
            var playerDataFile = new PlayerDataFile();
            playerDataFile.Load(GameIO.GetPlayerDataDir(), persistentPlayerData.UserIdentifier.CombinedString);

            if (playerDataFile.bLoaded == false)
            {
               return new HistoryPlayerDetails(persistentPlayerData.ToHistoryPlayer());
            }
            else
            {
                return playerDataFile.ToPlayerDetails<HistoryPlayerDetails>(persistentPlayerData);
            }
        }
    }
}
