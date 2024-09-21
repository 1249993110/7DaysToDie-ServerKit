namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Undo last prefab command.
    /// </summary>
    public class UndoPrefab : ConsoleCmdBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            return "Undo last prefab command.";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. ty-UndoPrefab\n" +
                "1. Undo prefabs command. Works with PlacePrefab, FillBlock, ReplaceBlock and DuplicateArea\n" +
                "NOTE: By default the size of undo history ise set to 1. You can change the undo history size using \"setundosize\"\n";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string[] getCommands()
        {
            return new string[]
            {
                "ty-UndoPrefab",
                "ty-undo",
                "ty-up"
            };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            try
            {
                var undoPrefab = GetUndoPrefab(senderInfo.GetEntityId());
                if (undoPrefab == null)
                {
                    Log("ERR: Unable to undo the last prefab command.");
                }
                else
                {
                    var prefab = undoPrefab.Prefab;
                    int prefabInstanceId = undoPrefab.PrefabInstanceId;
                    var prefabSize = prefab.size;
                    var offsetPosition = undoPrefab.OffsetPosition;

                    IEnumerable<Chunk>? chunks;
                    try
                    {
                        chunks = ChunkHelper.GetChunksInArea(offsetPosition, prefabSize);
                    }
                    catch (Exception ex)
                    {
                        Log("ERR: " + ex.Message);
                        return;
                    }

                    foreach (var chunk in chunks)
                    {
                        chunk.GetSleeperVolumes().Clear();
                    }

                    prefab.CopyIntoLocal(GameManager.Instance.World.ChunkCache, offsetPosition, true, true, FastTags<TagGroup.Global>.none);
                    
                    ChunkHelper.ForceReload(chunks);

                    ChunkHelper.CalculateStability(offsetPosition, prefabSize);

                    ChunkHelper.RemoveEntityInArea(offsetPosition.x, offsetPosition.z, offsetPosition.x + prefab.size.x, offsetPosition.z + prefab.size.z);

                    if (prefabInstanceId > 0)
                    {
                        if (ChunkHelper.RemovePrefabFromRWG(prefabInstanceId))
                        {
                            Log("Found undoBrender with \"addToRWG\". Removed prefab with PrefabInstanceId={0} from Randomgen World.", prefabInstanceId);
                        }
                    }

                    Log($"Prefab Undone at {offsetPosition}");
                }
            }
            catch (Exception ex)
            {
                Log("Error in UndoPrefab.Execute" + Environment.NewLine + ex.ToString());
            }
        }        

        internal static UndoPrefabObj? GetUndoPrefab(int entityId)
        {
            if (_prefabCache.IsValueCreated == false
                || _prefabCache.Value.TryGetValue(entityId, out var list) == false 
                || list.Count == 0)
            {
                return null;
            }

            var obj = list[0];
            list.RemoveAt(0);
            return obj;
        }

        internal static void SetUndo(int entityId, Prefab prefab, Vector3i offsetPosition, int prefabInstanceId)
        {
            if (_prefabCache.Value.TryGetValue(entityId, out var list) == false)
            {
                list = new List<UndoPrefabObj>();
                _prefabCache.Value.Add(entityId, list);
            }

            if (list.Count >= _maxUndoHistorySize)
            {
                list.RemoveAt(list.Count - 1);
            }

            var item = new UndoPrefabObj(prefab, offsetPosition, prefabInstanceId);
            list.Insert(0, item);
        }

        private static int _maxUndoHistorySize = 1;
        private static readonly Lazy<Dictionary<int, List<UndoPrefabObj>>> _prefabCache = new Lazy<Dictionary<int, List<UndoPrefabObj>>>();

        internal static void SetMaxUndoHistorySize(int maxUndoHistorySize, int entityId)
        {
            if(maxUndoHistorySize <= 0)
            {
                if (_prefabCache.IsValueCreated)
                {
                    if (entityId == -1)
                    {
                        _prefabCache.Value.Clear();
                    }
                    else
                    {
                        _prefabCache.Value.Remove(entityId);
                    }
                }
            }
            else
            {
                _maxUndoHistorySize = maxUndoHistorySize;
            }
        }

        internal static int GetMaxUndoHistorySize()
        {
            return _maxUndoHistorySize;
        }

        internal class UndoPrefabObj
        {
            public UndoPrefabObj(Prefab prefab, Vector3i offsetPosition, int prefabInstanceId)
            {
                this.Prefab = prefab;
                this.OffsetPosition = offsetPosition;
                this.PrefabInstanceId = prefabInstanceId;

                // Clear sleeper volumes
                prefab.SleeperVolumes.Clear();
            }

            public Prefab Prefab { get; set; }
            public Vector3i OffsetPosition { get; set; }
            public int PrefabInstanceId { get; set; }
        }
    }
}
