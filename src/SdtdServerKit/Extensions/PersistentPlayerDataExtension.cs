namespace SdtdServerKit.Extensions
{
    public static class PersistentPlayerDataExtension
    {
        public static HistoryPlayer ToHistoryPlayer(this PersistentPlayerData persistentPlayerData)
        {
            return new HistoryPlayer()
            {
                PlatformId = persistentPlayerData.NativeId.CombinedString,
                CrossplatformId = persistentPlayerData.PrimaryId.CombinedString,
                PlayerName = persistentPlayerData.PlayerName.DisplayName,
                Position = persistentPlayerData.Position.ToPosition(),
                LastLogin = persistentPlayerData.LastLogin,
            };
        }

        public static HistoryPlayerDetails ToHistoryPlayerDetails(this PersistentPlayerData persistentPlayerData)
        {
            var playerDataFile = new PlayerDataFile();
            playerDataFile.Load(GameIO.GetPlayerDataDir(), persistentPlayerData.PrimaryId.CombinedString);

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
