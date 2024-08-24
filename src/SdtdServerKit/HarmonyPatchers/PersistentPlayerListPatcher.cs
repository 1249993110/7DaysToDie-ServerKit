﻿using HarmonyLib;

namespace SdtdServerKit.HarmonyPatchers
{
    [HarmonyPatch(typeof(PersistentPlayerList))]
    internal class PersistentPlayerListPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PersistentPlayerList.CleanupPlayers))]
        public static bool CleanupPlayers(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}