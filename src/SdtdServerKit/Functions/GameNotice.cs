using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Hooks;
using SdtdServerKit.Variables;

namespace SdtdServerKit.Functions
{
    public class GameNotice : FunctionBase<GameNoticeSettings>
    {
        private readonly SubTimer _timer;
        private int _lastRotatingIndex;

        public GameNotice()
        {
            _timer = new SubTimer(SendRotatingNotice);
        }

        protected override void OnDisableFunction()
        {
            ModEventHook.PlayerSpawnedInWorld -= OnPlayerSpawnedInWorld;
            ModEventHook.SkyChanged -= OnSkyChanged;
        }

        protected override void OnEnableFunction()
        {
            ModEventHook.PlayerSpawnedInWorld += OnPlayerSpawnedInWorld;
            ModEventHook.SkyChanged += OnSkyChanged;
        }

        protected override void OnSettingsChanged()
        {
            _timer.Interval = Settings.RotatingInterval;
            _timer.IsEnabled = Settings.IsEnabled;
            _lastRotatingIndex = 0;
        }

        private void OnPlayerSpawnedInWorld(SpawnedPlayer spawnedPlayer)
        {
            try
            {
                switch (spawnedPlayer.RespawnType)
                {
                    // New player spawning
                    case Shared.Models.RespawnType.EnterMultiplayer:
                    // Old player spawning
                    case Shared.Models.RespawnType.JoinMultiplayer:
                        SendMessageToPlayer(spawnedPlayer.CrossplatformId, FormatCmd(Settings.WelcomeNotice, spawnedPlayer));
                        break;
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in GameNotice.PlayerSpawnedInWorld");
            }
        }

        private void SendRotatingNotice()
        {
            try
            {
                var rotatingNotices = Settings.RotatingNotices;
                if (rotatingNotices != null && rotatingNotices.Length > 0)
                {
                    if(_lastRotatingIndex >= rotatingNotices.Length)
                    {
                        _lastRotatingIndex = 0;
                    }

                    SendGlobalMessage(rotatingNotices[_lastRotatingIndex]);
                    _lastRotatingIndex++;
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in GameNotice.SendRotatingNotice");
            }
        }

        private void OnSkyChanged(SkyChanged changed)
        {
            try
            {
                var gameNoticeVariables = new GameNoticeVariables()
                {
                    BloodMoonDays = changed.BloodMoonDaysRemaining,
                    BloodMoonStartTime = changed.DuskHour + ":00",
                    BloodMoonEndTime = changed.DawnHour + ":00"
                };

                // 黎明
                if (changed.SkyChangeEventType == SkyChangeEventType.Dawn)
                {
                    if (changed.BloodMoonDaysRemaining == 0)
                    {
                        SendGlobalMessage(StringTemplate.Render(Settings.BloodMoonNotice2, gameNoticeVariables));
                    }
                    else
                    {
                        SendGlobalMessage(StringTemplate.Render(Settings.BloodMoonNotice1, gameNoticeVariables));
                    }
                }
                // 黄昏
                else if (changed.SkyChangeEventType == SkyChangeEventType.Dusk)
                {
                    if (changed.BloodMoonDaysRemaining == 0)
                    {
                        SendGlobalMessage(StringTemplate.Render(Settings.BloodMoonNotice3, gameNoticeVariables));
                    }
                    else
                    {
                        SendGlobalMessage(StringTemplate.Render(Settings.BloodMoonNotice1, gameNoticeVariables));
                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in GameNotice.OnSkyChanged");
            }
        }

    }
}