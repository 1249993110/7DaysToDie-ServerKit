namespace SdtdServerKit.Extensions
{
    public static class SpawnPositionExtension
    {
        public static SdtdServerKit.Shared.Models.SpawnPosition ToModel(this SpawnPosition spawnPosition)
        {
            return new SdtdServerKit.Shared.Models.SpawnPosition()
            {
                ClrIdx = spawnPosition.ClrIdx,
                Position = spawnPosition.position.ToPosition(),
                Heading = spawnPosition.heading,
            };
        }
    }
}
