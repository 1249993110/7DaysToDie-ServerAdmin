using HarmonyLib;

namespace LSTY.Sdtd.ServerAdmin.Patches.Harmony
{
    [HarmonyPatch(typeof(PersistentPlayerList), nameof(PersistentPlayerList.CleanupPlayers))]
    internal static class PreventPlayerCleanup_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}