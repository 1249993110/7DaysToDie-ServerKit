using HarmonyLib;
using Platform.Steam;
using SdtdServerKit.HarmonyPatchers;
using SdtdServerKit.Managers;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class GlobalSettings : FunctionBase<FunctionSettings.GlobalSettings>
    {
        private new FunctionSettings.GlobalSettings Settings => ConfigManager.GlobalSettings;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GlobalSettings()
        {
            ModEventHub.EntityKilled += OnEntityKilled;
            ModEventHub.PlayerSpawnedInWorld += OnPlayerSpawnedInWorld;
            ModEventHub.EntitySpawned += OnEntitySpawned;
        }

        private void OnEntitySpawned(EntityInfo entityInfo)
        {
            if (Settings.EnableAutoZombieCleanup)
            {
                int zombies = 0;
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity.IsAlive())
                    {
                        if (entity is EntityEnemy)
                        {
                            zombies++;
                        }
                    }
                }
                if(zombies > Settings.AutoZombieCleanupThreshold)
                {
                    Utilities.Utils.ExecuteConsoleCommand("ty-RemoveEntity " + entityInfo.EntityId, true);
                    CustomLogger.Info($"Auto zombie cleanup triggered, the entity: {entityInfo.EntityName} was removed.");
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnSettingsChanged()
        {
            { 
                var original = AccessTools.Method(typeof(GameManager), nameof(GameManager.ChangeBlocks));
                var patch = AccessTools.Method(typeof(GameManagerPatcher), nameof(GameManagerPatcher.Before_ChangeBlocks));

                if (Settings.RemoveSleepingBagFromPOI)
                {
                    ModApi.Harmony.Patch(original, prefix: new HarmonyMethod(patch));
                }
                else
                {
                    ModApi.Harmony.Unpatch(original, patch);
                }
            }

            {
                var original = AccessTools.Method(typeof(GameManager), nameof(GameManager.RequestToSpawnPlayer));
                var patch = AccessTools.Method(typeof(GameManagerPatcher), nameof(GameManagerPatcher.Before_RequestToSpawnPlayer));

                if (Settings.EnableXmlsSecondaryOverwrite)
                {
                    ModApi.Harmony.Patch(original, prefix: new HarmonyMethod(patch));
                }
                else
                {
                    ModApi.Harmony.Unpatch(original, patch);
                }
            }

            {
                var original = AccessTools.Method(typeof(PlayerDataFile), nameof(PlayerDataFile.ToPlayer));
                var patch = AccessTools.Method(typeof(PlayerDataFilePatcher), nameof(PlayerDataFilePatcher.After_ToPlayer));

                if (Settings.IsEnablePlayerInitialSpawnPoint)
                {
                    ModApi.Harmony.Patch(original, postfix: new HarmonyMethod(patch));
                }
                else
                {
                    ModApi.Harmony.Unpatch(original, patch);
                }
            }

            {
                var original = AccessTools.Method(typeof(World), nameof(World.AddFallingBlock));
                var patch = AccessTools.Method(typeof(WorldPatcher), nameof(WorldPatcher.Before_AddFallingBlock));
                if (Settings.EnableFallingBlockProtection)
                {
                    ModApi.Harmony.Patch(original, prefix: new HarmonyMethod(patch));
                }
                else
                {
                    ModApi.Harmony.Unpatch(original, patch);
                }
            }
        }

        private void BlockFamilySharingAccount(ClientInfo clientInfo)
        {
            if (clientInfo.PlatformId is UserIdentifierSteam userIdentifierSteam
                && userIdentifierSteam.OwnerId.Equals(userIdentifierSteam) == false)
            {
                Utilities.Utils.ExecuteConsoleCommand("kick " + clientInfo.entityId + " \"Family sharing account is not allowed to join the server!\"");
            }
        }

        private void OnPlayerSpawnedInWorld(SpawnedPlayer player)
        {
            if (Settings.BlockFamilySharingAccount)
            {
                if (player.RespawnType == Models.RespawnType.EnterMultiplayer
                    || player.RespawnType == Models.RespawnType.JoinMultiplayer)
                {
                    var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(player.EntityId);
                    BlockFamilySharingAccount(clientInfo);
                }
            }

            if (Settings.DeathTrigger.IsEnabled)
            {
                if (player.RespawnType == Models.RespawnType.Died)
                {
                    foreach (var command in Settings.DeathTrigger.ExecuteCommands)
                    {
                        if (string.IsNullOrEmpty(command) == false)
                        {
                            Utilities.Utils.ExecuteConsoleCommand(FormatCmd(command, player), true);
                        }
                    }
                }
            }
        }

        private void OnEntityKilled(KilledEntity entity)
        {
            if (Settings.KillZombieTrigger.IsEnabled)
            {
                if (entity.DeadEntity.EntityType == Models.EntityType.Zombie)
                {
                    var player = LivePlayerManager.GetByEntityId(entity.KillerEntityId);
                    foreach (var command in Settings.KillZombieTrigger.ExecuteCommands)
                    {
                        if (string.IsNullOrEmpty(command) == false)
                        {
                            Utilities.Utils.ExecuteConsoleCommand(FormatCmd(command, player), true);
                        }
                    }
                }
            }
        }
    }
}
