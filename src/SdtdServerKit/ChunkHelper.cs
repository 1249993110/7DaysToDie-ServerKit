using SdtdServerKit.Managers;

namespace SdtdServerKit
{
    /// <summary>
    /// Provides a set of helper methods for chunks.
    /// </summary>
    public static class ChunkHelper
    {
        /// <summary>
        /// Forces the reload of the specified chunks.
        /// </summary>
        /// <param name="chunks"></param>
        public static void ForceReload(IEnumerable<Chunk> chunks)
        {
            var managedPlayers = LivePlayerManager.GetAll();
            Parallel.ForEach(managedPlayers, managedPlayer =>
            {
                foreach (var chunk in chunks)
                {
                    try
                    {
                        var chunkPosition = chunk.GetWorldPos();
                        var playerPosition = managedPlayer.EntityPlayer.GetPosition();
                        if (Math.Abs(playerPosition.x - chunkPosition.x) < 200F && Math.Abs(playerPosition.z - chunkPosition.z) < 200F)
                        {
                            managedPlayer.ClientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunk>().Setup(chunk, true));
                        }
                    }
                    catch (Exception)
                    {
                        // CustomLogger.Warn(ex, "Error in ChunkHelper.ForceReload");
                    }
                }
            });

            ConnectionManager.Instance.FlushClientSendQueues();
        }
    }
}
