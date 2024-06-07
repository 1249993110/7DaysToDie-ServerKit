using SdtdServerKit.Hooks;
using Webserver.WebAPI.APIs.WorldState;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class GlobalSettings : FunctionBase<FunctionSettings.GlobalSettings>
    {
        public GlobalSettings()
        {
            ModEventHook.EntityKilled += OnEntityKilled;
            ModEventHook.PlayerSpawnedInWorld += PlayerSpawnedInWorld;
        }

        private void PlayerSpawnedInWorld(SpawnedPlayer player)
        {
            if(Settings.DeathTrigger.IsEnabled)
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
