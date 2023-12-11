namespace SdtdServerKit.Shared.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Language
    {
        Schinese,
        English
    }
}
