using HarmonyLib;
using System.Reflection;
using Webserver.WebAPI.APIs.WorldState;

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
                GetOriginal(typeof(GameManager), nameof(GameManager.ChangeBlocks)), 
                typeof(RemoveSleepingBagFromPOI),
                _prefixName: nameof(GameManager_ChangeBlocks_Prefix));

            CustomLogger.Info("Patch lpblock delete.");
        }
        public static void UnPatch()
        {
            PrivateUnpatch(
                GetOriginal(typeof(GameManager), nameof(GameManager.ChangeBlocks)), 
                typeof(RemoveSleepingBagFromPOI),
                _prefixName: nameof(GameManager_ChangeBlocks_Prefix));

            CustomLogger.Info("UnPatch lpblock delete.");
        }

        private static void GameManager_ChangeBlocks_Postfix(
            GameManager __instance,
            PlatformUserIdentifierAbs persistentPlayerId,
            List<BlockChangeInfo> _blocksToChange)
        {
            World world = __instance.World;
            foreach (var info in _blocksToChange)
            {
                var blockToChange = info.blockValue.Block;
                if (blockToChange is BlockLandClaim || blockToChange is BlockSleepingBag)
                {
                    var blockPosition = info.pos;
                    int num = GameStats.GetInt(EnumGameStats.ScoreZombieKillMultiplier) / 2;
                    int num2 = blockPosition.x - num;
                    int num3 = blockPosition.x + num;
                    int num4 = blockPosition.z - num;
                    int num5 = blockPosition.z + num;
                    var poiBulidings = new List<PrefabInstance>();
                    GameManager.Instance.World.GetPOIsAtXZ(num2, num3, num4, num5, poiBulidings);
                    if (poiBulidings.Count > 0)
                    {
                        //var persistentPlayerDataFromId = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(persistentPlayerId);
                        //persistentPlayerDataFromId.BedrollPos = new Vector3i(0, int.MaxValue, 0);
                        //BlockValue oldBlockValue = world.GetBlock(blockPosition);
                        //info.blockValue = oldBlockValue;
                        CustomLogger.Info(string.Format("Land claim detected on: {0}, {1}", blockPosition.x, blockPosition.z));

                        //string actionName = "remove_landclaims";
                        //GameEventManager.Current.HandleAction(actionName, null, player, false, "");

                        //var persistentPlayerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(persistentPlayerId);
                        //int entityId = persistentPlayerData.EntityId;


                        //ConnectionManager.Instance.SendPackage(
                        //    NetPackageManager.GetPackage<NetPackageGameEventResponse>()
                        //    .Setup(actionName, entityId, null, null, NetPackageGameEventResponse.ResponseTypes.ClientSequenceAction, -1, this.ActionIndex, false), 
                        //    false, entityId);

                        //cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>()
                        //    .Setup(actionName, cInfo.entityId, string.Empty, string.Empty, NetPackageGameEventResponse.ResponseTypes.Approved));
                    }

                    //NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(null, _blocksToChange, -1);
                    //foreach (var client in ConnectionManager.Instance.Clients.List)
                    //{
                    //    client.SendPackage(package);
                    //}
                }
            }
        }

        private static bool GameManager_ChangeBlocks_Prefix(
            GameManager __instance,
            PlatformUserIdentifierAbs persistentPlayerId,
            List<BlockChangeInfo> _blocksToChange)
        {
            if(persistentPlayerId == null)
            {
                return true;
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(persistentPlayerId);

            if(clientInfo == null)
            {
                return true;
            }

            World world = __instance.World;
            foreach (var info in _blocksToChange)
            {
                var blockToChange = info.blockValue.Block;
                if (blockToChange is BlockLandClaim || blockToChange is BlockSleepingBag)
                {
                    var blockPosition = info.pos;
                    int num = GameStats.GetInt(EnumGameStats.ScoreZombieKillMultiplier) >> 2;
                    int num2 = blockPosition.x - num;
                    int num3 = blockPosition.x + num;
                    int num4 = blockPosition.z - num;
                    int num5 = blockPosition.z + num;
                    var poiBulidings = new List<PrefabInstance>();
                    GameManager.Instance.World.GetPOIsAtXZ(num2, num3, num4, num5, poiBulidings);
                    if (poiBulidings.Count > 0)
                    {
                        BlockValue oldBlockValue = world.GetBlock(blockPosition);
                        info.blockValue = oldBlockValue;

                        NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(null, new List<BlockChangeInfo>() { info }, -1);
                        clientInfo.SendPackage(package);

                        CustomLogger.Info("Land claim detected on: {0}, {1}", blockPosition.x, blockPosition.z);
                    }
                }
            }

            return true;
        }
    }
}
