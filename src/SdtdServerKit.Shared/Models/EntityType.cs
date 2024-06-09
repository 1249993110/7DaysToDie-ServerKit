namespace SdtdServerKit.Shared.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityType
    {
        OfflinePlayer,
        OnlinePlayer = 1,
        Zombie,
        Animal,
        Bandit,

        // group
        Hostiles = -1
    }
}