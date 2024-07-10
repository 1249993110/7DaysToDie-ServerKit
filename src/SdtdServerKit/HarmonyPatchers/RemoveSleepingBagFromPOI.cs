using HarmonyLib;
using System.Reflection;

namespace SdtdServerKit.HarmonyPatchers
{
    internal class RemoveSleepingBagFromPOI
    {
        private static Harmony HarmonyPatch => ModApi.Harmony;

        private static void PrivatePatch(MethodInfo original, Type classType, string _prefixName = "", string _postfixName = "",
            string _transpiler = "", string _finalizer = "")
        {
            if (original == null)
            {
                return;
            }

            if (_prefixName != "")
            {
                var prefix = AccessTools.Method(classType, _prefixName);
                if (prefix == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _prefixName;
                    Log.Out(error);
                    return;
                }

                HarmonyPatch.Patch(original, new HarmonyMethod(prefix));
            }

            if (_postfixName != "")
            {
                var postfix = AccessTools.Method(classType, _postfixName);
                if (postfix == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _postfixName;
                    Log.Out(error);
                    return;
                }

                HarmonyPatch.Patch(original, null, new HarmonyMethod(postfix), null);
            }

            if (_transpiler != "")
            {
                var transpiler = AccessTools.Method(classType, _transpiler);
                if (transpiler == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _transpiler;
                    Log.Out(error);
                    return;
                }

                HarmonyPatch.Patch(original, null, null, new HarmonyMethod(transpiler));
            }

            if (_finalizer != "")
            {
                var finalizer = AccessTools.Method(classType, _finalizer);
                if (finalizer == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _finalizer;
                    Log.Out(error);
                    return;
                }

                HarmonyPatch.Patch(original, finalizer: new HarmonyMethod(finalizer));
            }
        }

        private static void PrivateUnpatch(MethodInfo original, Type classType, string _prefixName = "", string _postfixName = "",
            string _transpiler = "", string _finalizer = "")
        {
            if (original == null)
            {
                return;
            }

            if (_prefixName != "")
            {
                var prefix = AccessTools.Method(classType, _prefixName);
                if (prefix == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _prefixName;
                    CustomLogger.Error(error);
                    return;
                }

                HarmonyPatch.Unpatch(original, prefix);
            }

            if (_postfixName != "")
            {
                var postfix = AccessTools.Method(classType, _postfixName);
                if (postfix == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _postfixName;
                    CustomLogger.Error(error);
                    return;
                }

                HarmonyPatch.Unpatch(original, postfix);
            }

            if (_transpiler != "")
            {
                var transpiler = AccessTools.Method(classType, _transpiler);
                if (transpiler == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _transpiler;
                    CustomLogger.Error(error);
                    return;
                }

                HarmonyPatch.Unpatch(original, transpiler);
            }

            if (_finalizer != "")
            {
                var finalizer = AccessTools.Method(classType, _finalizer);
                if (finalizer == null)
                {
                    string error = "Injection failed: " + original.ToString() + "." + _finalizer;
                    CustomLogger.Error(error);
                    return;
                }

                HarmonyPatch.Unpatch(original, finalizer);
            }
        }

        private static MethodInfo GetOriginal(Type classType, string methodName, Type[] parameters = null)
        {
            var methodInfo = AccessTools.Method(classType, methodName, parameters);
            if (methodInfo != null)
            {
                return methodInfo;
            }

            string error = "Injection failed: " + classType.ToString() + "." + methodName;
            Log.Out(error);
            return null;
        }

        public static void Patch()
        {
            PrivatePatch(
                GetOriginal(typeof(GameManager), "ChangeBlocks"), 
                typeof(RemoveSleepingBagFromPOI),
                _prefixName: nameof(GameManager_ChangeBlocks_Prefix));

            CustomLogger.Info("Patch lpblock delete.");
        }
        public static void UnPatch()
        {
            PrivateUnpatch(
                GetOriginal(typeof(GameManager), "ChangeBlocks"), 
                typeof(RemoveSleepingBagFromPOI),
                _prefixName: nameof(GameManager_ChangeBlocks_Prefix));

            CustomLogger.Info("UnPatch lpblock delete.");
        }

        private static bool GameManager_ChangeBlocks_Prefix(
            GameManager __instance,
            PlatformUserIdentifierAbs persistentPlayerId,
            List<BlockChangeInfo> _blocksToChange)
        {
            World world = __instance.World;
            foreach (var info in _blocksToChange)
            {
                if (info == null)
                {
                    continue;
                }

                if (info.blockValue.Block.IndexName == "lpblock")
                {
                    var blockPosition = info.pos;
                    BlockValue oldBlockValue;
                    var vector = blockPosition.ToVector3();
                    float num = (float)GameStats.GetInt(EnumGameStats.ScoreZombieKillMultiplier) / 2F;
                    int num2 = (int)(vector.x - num);
                    int num3 = (int)(vector.x + num);
                    int num4 = (int)(vector.z - num);
                    int num5 = (int)(vector.z + num);
                    List<PrefabInstance> list2 = new List<PrefabInstance>();
                    GameManager.Instance.World.GetPOIsAtXZ(num2, num3, num4, num5, list2);
                    if (list2.Count > 0)
                    {
                        var persistentPlayerDataFromId = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(persistentPlayerId);
                        persistentPlayerDataFromId.BedrollPos = new Vector3i(0, int.MaxValue, 0);
                        oldBlockValue = world.GetBlock(blockPosition);
                        info.blockValue = oldBlockValue;
                        CustomLogger.Info(string.Format("Sleeping Bag detected on: {0}, {1}", blockPosition.x, blockPosition.z));
                    }
                    NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(null, _blocksToChange, -1);
                    foreach (var client in ConnectionManager.Instance.Clients.List)
                    {
                        client.SendPackage(package);
                    }
                    return true;
                }
            }
            return true;
        }
    }
}
