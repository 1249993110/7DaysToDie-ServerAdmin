using HarmonyLib;

namespace LSTY.Sdtd.ServerAdmin.HarmonyPatchers
{
    [HarmonyPatch(typeof(PersistentPlayerList))]
    internal static class PersistentPlayerListPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PersistentPlayerList.CleanupPlayers))]
        public static bool Before_CleanupPlayers(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}