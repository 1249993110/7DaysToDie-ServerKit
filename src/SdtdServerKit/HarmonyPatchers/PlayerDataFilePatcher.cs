using HarmonyLib;
using SdtdServerKit.Managers;

namespace SdtdServerKit.HarmonyPatchers
{
    [HarmonyPatch(typeof(PlayerDataFile))]
    internal class PlayerDataFilePatcher
    {
        public static void After_ToPlayer(PlayerDataFile __instance, EntityPlayer _player)
        {
            try
            {
                if (__instance.bLoaded == false)
                {
                    string initialPosition = ConfigManager.GlobalSettings.PlayerInitialPosition;
                    if (string.IsNullOrEmpty(initialPosition) == false)
                    {
                        string[] position = initialPosition.Split(' ');
                        if (position.Length == 3)
                        {
                            var pos = new UnityEngine.Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2]));
                            __instance.ecd.pos = pos;
                            _player.InitLocation(pos, __instance.ecd.rot);
                            CustomLogger.Warn(pos.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in PlayerDataFilePatcher.ToPlayer");
            }
        }
    }
}