namespace SdtdServerKit.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Language
    {
        Schinese,
        English
    }
}
