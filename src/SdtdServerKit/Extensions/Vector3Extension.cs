using UnityEngine;

namespace SdtdServerKit.Extensions
{
    internal static class Vector3Extension
    {
        public static Position ToPosition(this Vector3 v)
        {
            return new Position(v.x, v.y, v.z);
        }

        public static Position ToPosition(this Vector3i v)
        {
            return new Position(v.x, v.y, v.z);
        }

        public static IEnumerable<Position> ToPositions(this IEnumerable<Vector3> v)
        {
            return v.Select(i => i.ToPosition());
        }

        public static IEnumerable<Position> ToPositions(this IEnumerable<Vector3i> v)
        {
            return v.Select(i => i.ToPosition());
        }
    }
}