namespace SdtdServerKit.Triggers
{
    /// <summary>
    /// SkyChangeTrigger
    /// </summary>
    public static class SkyChangeTrigger
    {
        private static int _lastDays;
        private static bool _isDark;

        private static int DaysRemaining(int days)
        {
            int bloodmoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            int daysSinceLastBloodMoon = days % bloodmoonFrequency;

            if (daysSinceLastBloodMoon == 0)
            {
                return bloodmoonFrequency;
            }

            return bloodmoonFrequency - daysSinceLastBloodMoon;
        }

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
                int bloodMoonDaysRemaining = DaysRemaining(days);
                if (_isDark)
                {
                    if(hours == world.DuskHour)
                    {
                        ModEventHub.OnSkyChanged(new SkyChanged()
                        {
                            BloodMoonDaysRemaining = bloodMoonDaysRemaining,
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
                        ModEventHub.OnSkyChanged(new SkyChanged()
                        {
                            BloodMoonDaysRemaining = bloodMoonDaysRemaining,
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