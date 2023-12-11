using SdtdServerKit.Hooks;

namespace SdtdServerKit.Triggers
{
    /// <summary>
    /// SkyChangeTrigger
    /// </summary>
    public static class SkyChangeTrigger
    {
        private static int _lastDays;
        private static bool _isDark;

        /// <summary>
        /// Callback
        /// </summary>
        public static void Callback()
        {
            var world = GameManager.Instance.World;

            if (world == null)
            {
                return;
            }

            int days = GameUtils.WorldTimeToDays(world.GetWorldTime());
            bool isDark = world.IsDark();

            // 首次 或 跨天
            if(_lastDays == 0 || _lastDays != days)
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

                if (_isDark)
                {
                    if(hours == world.DuskHour)
                    {
                        ModEventHook.OnSkyChanged(new SkyChanged()
                        {
                            BloodMoonDaysRemaining = Utils.DaysRemaining(days),
                            DawnHour = world.DawnHour,
                            DuskHour = world.DuskHour,
                            SkyChangeEventType = SkyChangeEventType.Dusk
                        });
                    }
                }
                else
                {
                    if(hours == world.DawnHour)
                    {
                        ModEventHook.OnSkyChanged(new SkyChanged()
                        {
                            BloodMoonDaysRemaining = Utils.DaysRemaining(days),
                            DawnHour = world.DawnHour,
                            DuskHour = world.DuskHour,
                            SkyChangeEventType = SkyChangeEventType.Dawn
                        });
                    }
                }
            }
        }
    }
}