namespace SdtdServerKit.Shared.Enums
{
    /// <summary>
    /// ModEventType
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ModEventType
    {
        Welcome,
        GameAwake,
        GameStartDone,
        GameUpdate,
        GameShutdown,
        CalcChunkColorsDone,
        ChatMessage,
        EntityKilled,
        EntitySpawned,
        LogCallback,
        PlayerDisconnected,
        PlayerLogin,
        PlayerSpawnedInWorld,
        PlayerSpawning,
        SavePlayerData,
        SkyChanged,
        CommandExecutionReply
    }
}
