using Platform.Steam;
using SdtdServerKit.HarmonyPatchers;
using SdtdServerKit.Hooks;
using SdtdServerKit.Managers;
using System.Timers;
using Webserver.WebAPI.APIs.WorldState;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class GlobalSettings : FunctionBase<FunctionSettings.GlobalSettings>
    {
        private readonly SubTimer autoRestartTimer;
        public GlobalSettings()
        {
            ModEventHook.EntityKilled += OnEntityKilled;
            ModEventHook.PlayerSpawnedInWorld += OnPlayerSpawnedInWorld;
            autoRestartTimer = new SubTimer(AutoRestart, 5) { IsEnabled = true };
            GlobalTimer.RegisterSubTimer(autoRestartTimer);
        }

        protected override void OnSettingsChanged()
        {
            if (Settings.RemoveSleepingBagFromPOI)
            {
                RemoveSleepingBagFromPOI.Patch();
            }
            else
            {
                RemoveSleepingBagFromPOI.UnPatch();
            }
        }
        
        private void BlockFamilySharingAccount(ClientInfo clientInfo)
        {
            if (clientInfo.PlatformId is UserIdentifierSteam userIdentifierSteam
                && userIdentifierSteam.OwnerId.Equals(userIdentifierSteam) == false)
            {
                Utils.ExecuteConsoleCommand("kick " + clientInfo.entityId + " \"Family sharing account is not allowed to join the server!\"");
            }
        }

        private async void AutoRestart()
        {
            DateTime now = DateTime.Now;
            
            if (Settings.AutoRestart.IsEnabled 
                && now.Hour == Settings.AutoRestart.RestartHour 
                && now.Minute == Settings.AutoRestart.RestartMinute
                && ModApi.IsGameStartDone)
            {
                autoRestartTimer.IsEnabled = false;

                if (Settings.AutoRestart.Messages != null)
                {
                    foreach (var item in Settings.AutoRestart.Messages)
                    {
                        Utils.ExecuteConsoleCommand("say \"" + item + "\"", true);
                        await Task.Delay(1000);
                    }
                }

                Utils.ExecuteConsoleCommand("ty-rs", true);
            }
        }

        private void OnPlayerSpawnedInWorld(SpawnedPlayer player)
        {
            if (Settings.BlockFamilySharingAccount)
            {
                if(player.RespawnType == Shared.Models.RespawnType.EnterMultiplayer 
                    || player.RespawnType == Shared.Models.RespawnType.JoinMultiplayer)
                {
                    var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(player.EntityId);
                    BlockFamilySharingAccount(clientInfo);
                }
            }

            if (Settings.DeathTrigger.IsEnabled)
            {
                if (player.RespawnType == Shared.Models.RespawnType.Died) 
                {
                    foreach (var command in Settings.DeathTrigger.ExecuteCommands)
                    {
                        if (string.IsNullOrEmpty(command) == false)
                        {
                            Utils.ExecuteConsoleCommand(FormatCmd(command, player), true);
                        }
                    }
                }
            }
        }

        private void OnEntityKilled(KilledEntity entity)
        {
            if(Settings.KillZombieTrigger.IsEnabled)
            {
                if(entity.DeadEntity.EntityType == Shared.Models.EntityType.Zombie)
                {
                    var player = ConnectionManager.Instance.Clients.ForEntityId(entity.KillerEntityId).ToOnlinePlayer(); // Convert entity.KillerEntityId to OnlinePlayer
                    foreach (var command in Settings.KillZombieTrigger.ExecuteCommands)
                    {
                        if(string.IsNullOrEmpty(command) == false)
                        {
                            Utils.ExecuteConsoleCommand(FormatCmd(command, player), true);
                        }
                    }
                }
            }
        }
    }
}
