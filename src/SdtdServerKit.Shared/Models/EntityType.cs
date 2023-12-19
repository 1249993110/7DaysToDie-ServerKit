namespace SdtdServerKit.Shared.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityType
    {
        OfflinePlayer,
        OnlinePlayer,
        Zombie,
        Animal,
        Bandit,

        // group
        Hostiles = -1
    }
}