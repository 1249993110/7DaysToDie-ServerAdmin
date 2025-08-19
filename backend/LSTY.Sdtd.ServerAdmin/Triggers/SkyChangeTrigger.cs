using LSTY.Sdtd.ServerAdmin.Shared.EventArgs;
using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Triggers
{
    /// <summary>
    /// SkyChangeTrigger
    /// </summary>
    internal static class SkyChangeTrigger
    {
        private static int _lastDays;
        private static bool _isDark;

        private static Timer? _timer;
        private static Action<SkyChangedEventArgs>? _skyChangedCallback;

        public static void Init(Action<SkyChangedEventArgs> skyChangedCallback)
        {
            _skyChangedCallback = skyChangedCallback;
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private static int DaysRemaining(int days)
        {
            int bloodmoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            int daysSinceLastBloodMoon = days % bloodmoonFrequency;

            return bloodmoonFrequency - daysSinceLastBloodMoon;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void TimerCallback(object? state)
        {
            var world = GameManager.Instance.World;

            if (world == null)
            {
                return;
            }

            int days = GameUtils.WorldTimeToDays(world.GetWorldTime());
            bool isDark = world.IsDark();

            // First time or cross day
            if (_lastDays == 0 || _lastDays != days)
            {
                _isDark = isDark;
                _lastDays = days;
                return;
            }

            if (_isDark == isDark)
            {
                return;
            }
            else
            {
                _isDark = isDark;

                int hours = GameUtils.WorldTimeToHours(world.GetWorldTime());
                int bloodMoonDaysRemaining = DaysRemaining(days);
                if (_isDark)
                {
                    if (hours == world.DuskHour)
                    {
                        var latestSkyState = new SkyChangedEventArgs()
                        {
                            BloodMoonDaysRemaining = bloodMoonDaysRemaining,
                            DawnHour = world.DawnHour,
                            DuskHour = world.DuskHour,
                            Type = SkyChangeEventType.Dusk,
                            Timestamp = DateTime.UtcNow
                        };
                        _skyChangedCallback?.Invoke(latestSkyState);
                    }
                }
                else
                {
                    if (hours == world.DawnHour)
                    {
                        var latestSkyState = new SkyChangedEventArgs()
                        {
                            BloodMoonDaysRemaining = bloodMoonDaysRemaining,
                            DawnHour = world.DawnHour,
                            DuskHour = world.DuskHour,
                            Type = SkyChangeEventType.Dawn,
                            Timestamp = DateTime.UtcNow
                        };
                        _skyChangedCallback?.Invoke(latestSkyState);
                    }
                }
            }
        }
    }
}