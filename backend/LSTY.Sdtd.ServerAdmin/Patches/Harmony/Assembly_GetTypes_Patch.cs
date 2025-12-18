using HarmonyLib;
using System.Reflection;

namespace LSTY.Sdtd.ServerAdmin.Patches.Harmony
{
    [HarmonyPatch]
    internal static class Assembly_GetTypes_Patch
    {
        private static readonly Func<Assembly, bool, Type[]> _originalGetTypes =
            AccessTools.MethodDelegate<Func<Assembly, bool, Type[]>>(typeof(Assembly)
                .GetMethod(nameof(Assembly.GetTypes), BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(bool) }, null));

        [HarmonyTargetMethod]
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(int).Assembly.GetType(), nameof(Assembly.GetTypes), new Type[] { });
        }

        [HarmonyPrefix]
        private static bool Prefix(Assembly __instance, ref Type[] __result)
        {
            try
            {
                __result = _originalGetTypes.Invoke(__instance, false);
                return false;
            }
            catch (ReflectionTypeLoadException ex)
            {
                __result = ex.Types.Where(t => t != null).ToArray();
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
