using SdtdServerKit.Managers;
using static SaveDataPrefsFile;

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

        /// <summary>
        /// Get chunks in area.
        /// </summary>
        /// <param name="offsetPosition"></param>
        /// <param name="prefabSize"></param>
        public static IEnumerable<Chunk> GetChunksInArea(Vector3i offsetPosition, Vector3i prefabSize)
        {
            var chunkSet = new HashSet<Chunk>();
            for (int i = 0; i < prefabSize.x; i++)
            {
                for (int j = 0; j < prefabSize.z; j++)
                {
                    for (int k = 0; k < prefabSize.y; k++)
                    {
                        int x = offsetPosition.x + i;
                        int y = offsetPosition.y + k;
                        int z = offsetPosition.z + j;
                        if (GameManager.Instance.World.IsChunkAreaLoaded(x, y, z) == false)
                        {
                            throw new Exception("The prefab is too far away. Chunk not loaded on that area.");
                        }

                        var chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(x, y, z);
                        chunkSet.Add(chunk);
                    }
                }
            }

            return chunkSet;
        }

        /// <summary>
        /// Calculate stability
        /// </summary>
        /// <param name="offsetPosition"></param>
        /// <param name="prefabSize"></param>
        public static void CalculateStability(Vector3i offsetPosition, Vector3i prefabSize)
        {
            var stabilityCalculator = new StabilityCalculator();
            stabilityCalculator.Init(GameManager.Instance.World);
            for (int i = 0; i < prefabSize.x; i++)
            {
                for (int j = 0; j < prefabSize.z; j++)
                {
                    for (int k = 0; k < prefabSize.y; k++)
                    {
                        var vector3i = new Vector3i(offsetPosition.x + i, offsetPosition.y + k, offsetPosition.z + j);
                        if (GameManager.Instance.World.GetBlock(vector3i).Equals(BlockValue.Air) == false)
                        {
                            stabilityCalculator.BlockPlacedAt(vector3i, false);
                        }
                    }
                }
            }
            stabilityCalculator.Cleanup();
        }

        /// <summary>
        /// Add prefab to RWG
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="offsetPosition"></param>
        /// <returns>Prefab instance id</returns>
        public static int AddPrefabToRWG(Prefab prefab, Vector3i offsetPosition)
        {
            var dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
            int prefabInstanceId = dynamicPrefabDecorator.GetNextId();
            var prefabInstance = new PrefabInstance(prefabInstanceId, prefab.location, offsetPosition, (byte)prefab.GetLocalRotation(), prefab, 0)
            {
                bPrefabCopiedIntoWorld = true
            };
            dynamicPrefabDecorator.GetDynamicPrefabs().Add(prefabInstance);
            dynamicPrefabDecorator.GetPOIPrefabs().Add(prefabInstance);
            dynamicPrefabDecorator.Save(PathAbstractions.WorldsSearchPaths.GetLocation(GameManager.Instance.World.ChunkCache.Name, null, null).FullPath);
            return prefabInstanceId;
        }

        /// <summary>
        /// Remove prefab from RWG
        /// </summary>
        /// <param name="prefabInstanceId"></param>
        /// <returns></returns>
        public static bool RemovePrefabFromRWG(int prefabInstanceId)
        {
            var dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
            var dynamicPrefabs = dynamicPrefabDecorator.GetDynamicPrefabs();
            var POIPrefabs = dynamicPrefabDecorator.GetPOIPrefabs();
            foreach (var prefabInstance in dynamicPrefabs)
            {
                if (prefabInstance != null && prefabInstance.id == prefabInstanceId)
                {
                    dynamicPrefabs.Remove(prefabInstance);
                    POIPrefabs.Remove(prefabInstance);
                    dynamicPrefabDecorator.Save(PathAbstractions.WorldsSearchPaths.GetLocation(GameManager.Instance.World.ChunkCache.Name, null, null).FullPath);
                    return true;
                }
            }

            return false;
        }
    }
}
