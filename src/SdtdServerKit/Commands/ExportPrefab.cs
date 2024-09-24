using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Exports an Area to the directory {UserDataFolder}/LocalPrefabs.
    /// </summary>
    public class ExportPrefab : ConsoleCmdBase
    {
        /// <inheritdoc/>
        public override string getDescription()
        {
            return "Exports an Area to the directory {UserDataFolder}/LocalPrefabs.";
        }

        /// <inheritdoc/>
        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. ty-ExportPrefab {x1} {y1} {z1} {x2} {y2} {z2} {prefab_file_name} [overwrite]\n" +
                "  2. ty-ExportPrefab \n" +
                "  3. ty-ExportPrefab {prefab_file_name} [overwrite]\n" +
                "1. Export the defined area to a prefabFile in folder {UserDataFolder}/LocalPrefabs\n" +
                "2. Store the player position to be used togheter on method 3.\n" +
                "3. Use stored position on method 2. with current position to export the area to prefab File in folder {UserDataFolder}/LocalPrefabs\n" +
                "NOTE: Sleepervolumes are lost during this process.";
        }

        /// <inheritdoc/>
        public override string[] getCommands()
        {
            return new string[]
            {
                "ty-ExportPrefab",
                "ty-ep"
            };
        }

        /// <inheritdoc/>
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 8 && _params.Count != 7 && _params.Count != 0 && _params.Count != 1 && _params.Count != 2)
                {
                    Log("ERR: Wrong number of arguments, expected 0 or 1, 2, 7 or 8, found " + _params.Count.ToString() + ".");
                    Log(this.GetHelp());
                }
                else
                {
                    int num = int.MinValue;
                    int num2 = int.MinValue;
                    int num3 = int.MinValue;
                    int num4 = int.MinValue;
                    int num5 = int.MinValue;
                    int num6 = int.MinValue;
                    string text = "";
                    if (_params.Count != 0)
                    {
                        if (_params.Count != 1)
                        {
                            if (_params.Count != 2)
                            {
                                if (_params.Count == 7 || _params.Count == 8)
                                {
                                    int.TryParse(_params[0], out num);
                                    int.TryParse(_params[2], out num2);
                                    int.TryParse(_params[4], out num3);
                                    int.TryParse(_params[1], out num4);
                                    int.TryParse(_params[3], out num5);
                                    int.TryParse(_params[5], out num6);
                                    text = _params[6];
                                    goto IL_23A;
                                }
                                goto IL_23A;
                            }
                        }
                        ClientInfo remoteClientInfo = _senderInfo.RemoteClientInfo;
                        if (remoteClientInfo == null)
                        {
                            Log("ERR: Unable to get your position");
                            return;
                        }
                        EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[remoteClientInfo.entityId];
                        if (entityPlayer == null)
                        {
                            Log("ERR: Unable to get your position");
                            return;
                        }
                        if (!PrefabExport.dictionary_0.ContainsKey(remoteClientInfo.entityId))
                        {
                            Log("ERR: There isnt any stored location. Use method 2. to store a position.");
                            Log(this.GetHelp());
                            return;
                        }
                        Vector3i vector3i;
                        PrefabExport.dictionary_0.TryGetValue(remoteClientInfo.entityId, out vector3i);
                        PrefabExport.dictionary_0.Remove(remoteClientInfo.entityId);
                        num = vector3i.x;
                        num2 = vector3i.y;
                        num3 = vector3i.z;
                        num4 = entityPlayer.GetBlockPosition().x;
                        num5 = entityPlayer.GetBlockPosition().y;
                        num6 = entityPlayer.GetBlockPosition().z;
                        text = _params[0];
                    IL_23A:
                        if (num != -2147483648)
                        {
                            if (num2 != -2147483648)
                            {
                                if (num3 != -2147483648)
                                {
                                    if (num4 != -2147483648)
                                    {
                                        if (num5 != -2147483648)
                                        {
                                            if (num6 != -2147483648)
                                            {
                                                if (num4 < num)
                                                {
                                                    int num7 = num;
                                                    num = num4;
                                                    num4 = num7;
                                                }
                                                if (num5 < num2)
                                                {
                                                    int num8 = num2;
                                                    num2 = num5;
                                                    num5 = num8;
                                                }
                                                if (num6 < num3)
                                                {
                                                    int num9 = num3;
                                                    num3 = num6;
                                                    num6 = num9;
                                                }
                                                if (!Extensions.ContainsCaseInsensitive(_params, "overwrite") && Prefab.PrefabExists(text))
                                                {
                                                    Log("A prefab with the name \"" + text + "\" already exists.");
                                                    return;
                                                }
                                                Prefab prefab = new Prefab();
                                                string str = text + ".tts";
                                                PathAbstractions.AbstractedLocation location;
                                                location..ctor(2, text, LaunchPrefs.UserDataFolder.Value + "/LocalPrefabs/" + str, null, true, null);
                                                prefab.location = location;
                                                prefab.copyFromWorld(GameManager.Instance.World, new Vector3i(num, num2, num3), new Vector3i(num4, num5, num6));
                                                prefab.bCopyAirBlocks = true;
                                                if (!prefab.Save(prefab.location, true))
                                                {
                                                    Log("Prefab could not be saved");
                                                    return;
                                                }
                                                Log(string.Concat(new string[]
                                                {
                                                    "Prefab ",
                                                    text,
                                                    " exported. Area mapped from ",
                                                    num.ToString(),
                                                    " ",
                                                    num2.ToString(),
                                                    " ",
                                                    num3.ToString(),
                                                    " to ",
                                                    num4.ToString(),
                                                    " ",
                                                    num5.ToString(),
                                                    " ",
                                                    num6.ToString()
                                                }));
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Log("ERR: At least one of the given coordinates is not a valid integer");
                    }
                    else
                    {
                        ClientInfo remoteClientInfo2 = _senderInfo.RemoteClientInfo;
                        if (remoteClientInfo2 != null)
                        {
                            EntityPlayer entityPlayer2 = GameManager.Instance.World.Players.dict[remoteClientInfo2.entityId];
                            if (!(entityPlayer2 == null))
                            {
                                if (PrefabExport.dictionary_0.ContainsKey(remoteClientInfo2.entityId))
                                {
                                    PrefabExport.dictionary_0.Remove(remoteClientInfo2.entityId);
                                }
                                PrefabExport.dictionary_0.Add(remoteClientInfo2.entityId, new Vector3i(entityPlayer2.GetBlockPosition().x, entityPlayer2.GetBlockPosition().y, entityPlayer2.GetBlockPosition().z));
                                SdtdConsole instance = SingletonMonoBehaviour<SdtdConsole>.Instance;
                                string[] array = new string[6];
                                array[0] = "Stored position: ";
                                int num10 = 1;
                                Vector3i blockPosition = entityPlayer2.GetBlockPosition();
                                array[num10] = blockPosition.x.ToString();
                                array[2] = " ";
                                int num11 = 3;
                                blockPosition = entityPlayer2.GetBlockPosition();
                                array[num11] = blockPosition.y.ToString();
                                array[4] = " ";
                                int num12 = 5;
                                blockPosition = entityPlayer2.GetBlockPosition();
                                array[num12] = blockPosition.z.ToString();
                                instance.Output(string.Concat(array));
                            }
                            else
                            {
                                Log("ERR: Unable to get your position");
                            }
                        }
                        else
                        {
                            Log("ERR: Unable to get your position");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str2 = "Error in PrefabExport.Run: ";
                Exception ex2 = ex;
                Log.Out(str2 + ((ex2 != null) ? ex2.ToString() : null));
            }
        }

        // Token: 0x04000235 RID: 565
        private static Dictionary<int, Vector3i> dictionary_0 = new Dictionary<int, Vector3i>();
    }
}
