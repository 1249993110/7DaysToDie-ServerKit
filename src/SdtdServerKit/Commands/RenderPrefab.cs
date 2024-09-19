﻿using SdtdServerKit.Managers;
using System.Drawing;
using UniLinq;

namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Renders a Prefab on given location.
    /// </summary>
    public class RenderPrefab : ConsoleCmdBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            return "Renders a Prefab on given location.";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. ty-RenderPrefab {prefabFileName} {x} {y} {z} [noSleepers] [addToRWG]\n" +
                "  2. ty-RenderPrefab {prefabFileName} {x} {y} {z} {rot} [noSleepers] [addToRWG]\n" +
                "  3. ty-RenderPrefab {prefabFileName} [noSleepers] [addToRWG]\n" +
                "  4. ty-RenderPrefab {prefabFileName} {rot} [noSleepers] [addToRWG]\n" +
                "  5. ty-RenderPrefab {prefabFileName} {rot} {depth} [noSleepers] [addToRWG]\n" +
                "1. Render prefab on {x} {y} {z} location\n" +
                "2. Render prefab on {x} {y} {z} location with rot\n" +
                "3. Render prefab on your position\n" +
                "4. Render prefab on your position with rot\n" +
                "5. Render prefab on your position with rot and y deslocated (depth blocks)\n" +
                "NOTE: {rot} means rotate the prefab to the left, must be equal to 0=0°, 1=90°, 2=180° or 3=270°\n" +
                "NOTE: Sleeper control is ONLY possible on prefabs that are present in prefabs.xml (world folder) that is used to create the map (RWG).\n" +
                "NOTE: Use parameter \"addToRWG\" to permanently add this prefab to the current RWG world. Can be reset like any other RWG prefab and will still be in world after a wipe. Will cause re-download of world for clients!";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string[] getCommands()
        {
            return new string[]
            {
                "ty-RenderPrefab",
                "ty-rp",
                "ty-brender"
            };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            try
            {
                if (args.Count < 1 || args.Count > 7)
                {
                    Log($"ERR: Wrong number of arguments, expected 1 to 7, found {args.Count}.");
                    Log(this.GetHelp());
                    return;
                }

                bool addToRWG = ContainsCaseInsensitive(args, "addtorwg");
                bool noSleepers = ContainsCaseInsensitive(args, "nosleepers");
                if (addToRWG)
                {
                    args.RemoveAll(i => string.Equals(i, "addtorwg", StringComparison.OrdinalIgnoreCase));
                }
                if (noSleepers)
                {
                    args.RemoveAll(i => string.Equals(i, "nosleepers", StringComparison.OrdinalIgnoreCase));
                }

                string prefabFileName = args[0];
                int x;
                int y;
                int z;
                int rot = 0;

                if (args.Count == 4)
                {
                    x = int.Parse(args[1]);
                    y = int.Parse(args[2]);
                    z = int.Parse(args[3]);
                }
                else if(args.Count == 5)
                {
                    x = int.Parse(args[1]);
                    y = int.Parse(args[2]);
                    z = int.Parse(args[3]);
                    rot = int.Parse(args[4]);
                }
                else
                {
                    var remoteClientInfo = senderInfo.RemoteClientInfo;
                    if (remoteClientInfo == null)
                    {
                        Log("ERR: This command can be only sent by player in game.");
                        return;
                    }
                    if (LivePlayerManager.TryGetByEntityId(remoteClientInfo.entityId, out var managedPlayer) == false)
                    {
                        Log("ERR: Unable to get your position");
                        return;
                    }

                    var playerBlockPosition = managedPlayer!.EntityPlayer.GetBlockPosition();
                    x = playerBlockPosition.x;
                    y = playerBlockPosition.y;
                    z = playerBlockPosition.z;

                    if(args.Count == 2)
                    {
                        rot = int.Parse(args[1]);
                        if (rot < 0 || rot > 3)
                        {
                            Log("ERR: Invalid rotation parameter. It need to be 0,1,2 or 3.");
                            return;
                        }
                    }
                    else if (args.Count == 3)
                    {
                        int depth = int.Parse(args[2]);
                        y += depth;
                    }
                }

                var prefab = new Prefab();
                if (prefab.Load(prefabFileName, true, true, false, false) == false)
                {
                    // Runtime load from LocalPrefabs
                    var dir = new DirectoryInfo(Path.Combine(LaunchPrefs.UserDataFolder.Value, "LocalPrefabs"));
                    Log("Try loading prefab from " + dir.FullName);
                    var abstractedLocation = new PathAbstractions.AbstractedLocation(PathAbstractions.EAbstractedLocationType.UserDataPath, prefabFileName, dir.FullName, null, prefabFileName, ".tts", true, null);
                    if (prefab.Load(abstractedLocation, true, true, false, false) == false)
                    {
                        Log("ERR: Unable to load prefab " + prefabFileName);
                        return;
                    }
                }

                Log("Rendering..., please wait.");

                prefab.bCopyAirBlocks = true;
                y += prefab.yOffset;
                prefab.RotateY(true, rot);
                var prefabSize = prefab.size;

                var chunkSet = new HashSet<Chunk>();
                for (int i = 0; i < prefabSize.x; i++)
                {
                    for (int j = 0; j < prefabSize.z; j++)
                    {
                        for (int k = 0; k < prefabSize.y; k++)
                        {
                            if (GameManager.Instance.World.IsChunkAreaLoaded(x + i, y + k, z + j) == false)
                            {
                                Log("ERR: The prefab is too far away. Chunk not loaded on that area.");
                                return;
                            }

                            var chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(x + i, y + k, z + j);
                            chunkSet.Add(chunk);
                        }
                    }
                }

                //var prefab2 = new Prefab(new Vector3i(prefabSize.x, prefabSize.y, prefabSize.z));
                //prefab2.bCopyAirBlocks = true;
                //prefab2.copyFromWorld(GameManager.Instance.World, new Vector3i(x, y, z), new Vector3i(x + prefabSize.x, y + prefabSize.y, z + prefabSize.z));
                if (noSleepers)
                {
                    prefab.SleeperVolumes = new List<Prefab.PrefabSleeperVolume>();
                }
                prefab.CopyIntoLocal(GameManager.Instance.World.ChunkCache, new Vector3i(x, y, z), true, true, FastTags<TagGroup.Global>.none);

                ChunkHelper.ForceReload(chunkSet);

                var stabilityCalculator = new StabilityCalculator();
                stabilityCalculator.Init(GameManager.Instance.World);
                for (int l = 0; l < prefabSize.x; l++)
                {
                    for (int m = 0; m < prefabSize.z; m++)
                    {
                        for (int n = 0; n < prefabSize.y; n++)
                        {
                            if (GameManager.Instance.World.GetBlock(x + l, y + n, z + m).Equals(BlockValue.Air) == false)
                            {
                                var vector3i = new Vector3i(x + l, y + n, z + m);
                                stabilityCalculator.BlockPlacedAt(vector3i, false);
                            }
                        }
                    }
                }
                stabilityCalculator.Cleanup();

                if (addToRWG)
                {
                    var dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
                    var prefabInstance = new PrefabInstance(dynamicPrefabDecorator.GetNextId(), prefab.location, new Vector3i(x, y, z), (byte)prefab.GetLocalRotation(), prefab, 0)
                    {
                        bPrefabCopiedIntoWorld = true
                    };
                    dynamicPrefabDecorator.GetDynamicPrefabs().Add(prefabInstance);
                    dynamicPrefabDecorator.GetPOIPrefabs().Add(prefabInstance);
                    dynamicPrefabDecorator.Save(PathAbstractions.WorldsSearchPaths.GetLocation(GameManager.Instance.World.ChunkCache.Name, null, null).FullPath);
                    //if (senderInfo.RemoteClientInfo == null)
                    //{
                    //    PrefabUndo.setUndo("server_", prefab2, new Vector3i(x, y, z), prefabInstance.id);
                    //}
                    //else
                    //{
                    //    PrefabUndo.setUndo(senderInfo.RemoteClientInfo.entityId.ToString() ?? "", prefab2, new Vector3i(x, y, z), prefabInstance.id);
                    //}
                }
                //else if (senderInfo.RemoteClientInfo == null)
                //{
                //    PrefabUndo.setUndo("server_", prefab2, new Vector3i(x, y, z), -1);
                //}
                //else
                //{
                //    PrefabUndo.setUndo(senderInfo.RemoteClientInfo.entityId.ToString() ?? "", prefab2, new Vector3i(x, y, z), -1);
                //}
                Log(string.Concat(new string[]
                {
                    "Prefab ",
                    prefabFileName,
                    " loaded at ",
                    x.ToString(),
                    " ",
                    y.ToString(),
                    " ",
                    z.ToString()
                }));
            }
            catch (Exception ex)
            {
                Log("Error in RenderPrefab.Execute" + Environment.NewLine + ex.ToString());
            }
        }
    }
}
