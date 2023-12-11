namespace SdtdServerKit.Shared.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityType
    {
        All,
        Player,
        Zombie,
        Animal,
        Bandit,

        // group
        Hostiles = -1
    }
}