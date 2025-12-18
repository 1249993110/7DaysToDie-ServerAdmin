using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Shared.EventArgs;

using UnityEngine;

namespace LSTY.Sdtd.ServerAdmin.Hooks
{
    [HarmonyPatch(typeof(World), nameof(World.OnUpdateTick))]
    internal static class SkyStateHook
    {
        public static event Action<SkyChangedEventArgs>? OnSkyChanged;

        private static float _timeAccumulator = 0F;
        private static int _lastDays = -1;
        private static bool _isDark = false;
        private const float CheckInterval = 1.0F;

        [HarmonyPostfix]
        private static void Postfix(World __instance)
        {
            if (__instance == null)
            {
                return;
            }

            _timeAccumulator += Time.deltaTime;
            if (_timeAccumulator < CheckInterval)
            {
                return;
            }

            _timeAccumulator = 0f;

            CheckSkyState(__instance);
        }

        private static void CheckSkyState(World world)
        {
            ulong worldTime = world.GetWorldTime();
            int days = GameUtils.WorldTimeToDays(worldTime);
            bool isDark = world.IsDark();

            // First time or cross day
            if (_lastDays == -1 || _lastDays != days)
            {
                _isDark = isDark;
                _lastDays = days;
                return;
            }

            if (_isDark == isDark)
            {
                return;
            }

            _isDark = isDark;

            var eventArgs = new SkyChangedEventArgs()
            {
                BloodMoonDaysRemaining = GetBloodMoonDaysRemaining(days),
                DawnHour = world.DawnHour,
                DuskHour = world.DuskHour,
                Type = _isDark ? SkyChangeEventType.Dusk : SkyChangeEventType.Dawn,
                Timestamp = DateTime.UtcNow,
                GameTime = new GameTimeDto()
                {
                    Days = GameUtils.WorldTimeToDays(worldTime),
                    Hours = GameUtils.WorldTimeToHours(worldTime),
                    Minutes = GameUtils.WorldTimeToMinutes(worldTime),
                },
            };

            OnSkyChanged?.Invoke(eventArgs);
        }

        private static int GetBloodMoonDaysRemaining(int days)
        {
            int bloodmoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            if (bloodmoonFrequency == 0) return -1;
            int daysSinceLastBloodMoon = days % bloodmoonFrequency;
            return bloodmoonFrequency - daysSinceLastBloodMoon;
        }
    }
}
