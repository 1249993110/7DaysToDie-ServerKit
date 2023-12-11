using UnityEngine;

namespace SdtdServerKit.Extensions
{
    internal static class ColorExtension
    {
        public static string ToHex(this Color color)
        {
            return string.Format("{0:X02}{1:X02}{2:X02}", (int)(color.r * 255), (int)(color.g * 255),
                (int)(color.b * 255));
        }
    }
}
