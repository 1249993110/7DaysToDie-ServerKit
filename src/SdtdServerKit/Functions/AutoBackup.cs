using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Hooks;
using SdtdServerKit.Variables;
using System.CodeDom.Compiler;
using System.IO.Compression;
using System.IO;
using Microsoft.Owin.FileSystems;
using UnityEngine;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 自动备份
    /// </summary>
    public class AutoBackup : FunctionBase<AutoBackupSettings>
    {
        private readonly SubTimer _timer;
        private DateTime _lastServerStateChange = DateTime.Now;

        /// <inheritdoc/>
        public AutoBackup()
        {
            _timer = new SubTimer(TryBackup);
        }

        /// <inheritdoc/>
        protected override void OnDisableFunction()
        {
            GlobalTimer.UnregisterSubTimer(_timer);
            ModEventHook.PlayerSpawnedInWorld -= OnPlayerSpawnedInWorld;
            ModEventHook.PlayerDisconnected -= OnPlayerDisconnected;
        }
        /// <inheritdoc/>
        protected override void OnEnableFunction()
        {
            GlobalTimer.RegisterSubTimer(_timer);
            ModEventHook.PlayerSpawnedInWorld += OnPlayerSpawnedInWorld;
            ModEventHook.PlayerDisconnected += OnPlayerDisconnected;
        }

        /// <inheritdoc/>
        protected override void OnSettingsChanged()
        {
            _timer.Interval = Settings.Interval;
            _timer.IsEnabled = Settings.IsEnabled;
        }
        private void OnPlayerDisconnected(OnlinePlayer player)
        {
            _lastServerStateChange = DateTime.Now;
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
                        _lastServerStateChange = DateTime.Now;
                        break;
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in AutoBackup.PlayerSpawnedInWorld");
            }
        }

        private void TryBackup()
        {
            try
            {
                if(ModApi.IsGameStartDone == false)
                {
                    return;
                }

                DateTime now = DateTime.Now;
                if(Settings.SkipIfThereAreNoPlayers 
                    && (now - _lastServerStateChange).TotalSeconds > Settings.Interval)
                {
                    CustomLogger.Info("AutoBackup: Skipped because there are no players.");
                    return;
                }

                ExecuteInternal();
            }
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in AutoBackup.TryBackup");
            }
        }

        private void ExecuteInternal()
        {
            string backupSrcPath = GameIO.GetSaveGameDir();
            string backupDestPath = Path.Combine(AppContext.BaseDirectory, Settings.ArchiveFolder);
            Directory.CreateDirectory(backupDestPath);

            // 服务端版本、游戏世界、游戏名称、游戏时间
            string serverVersion = Constants.cVersionInformation.LongString;
            string gameWorld = GamePrefs.GetString(EnumGamePrefs.GameWorld);
            string gameName = GamePrefs.GetString(EnumGamePrefs.GameName);

            var worldTime = GameManager.Instance.World.GetWorldTime();
            int days = GameUtils.WorldTimeToDays(worldTime);
            int hours = GameUtils.WorldTimeToHours(worldTime);
            int minutes = GameUtils.WorldTimeToMinutes(worldTime);

            string title = $"{serverVersion}_{gameWorld}_{gameName}_Day{days}_Hour{hours}";
            string archiveFileName = Path.Combine(backupDestPath, $"{title}.zip");

            if (File.Exists(archiveFileName))
            {
                CustomLogger.Info("AutoBackup: Backup already exists: {0}", archiveFileName);
                return;
            }

            ZipFile.CreateFromDirectory(backupSrcPath, archiveFileName, System.IO.Compression.CompressionLevel.Optimal, true);
            CustomLogger.Info("AutoBackup: Backup created: {0}", archiveFileName);

            if (Settings.RetainedFileCountLimit > 0)
            {
                string[] files = Directory.GetFiles(backupDestPath, "*.zip");
                int count = files.Length - Settings.RetainedFileCountLimit;
                if (count > 0)
                {
                    // 根据文件的创建日期对文件进行排序
                    var oldestFiles = files.Select(i => new FileInfo(i)).OrderBy(f => f.CreationTime).Take(count);
                    foreach (var oldestFile in oldestFiles)
                    {
                        CustomLogger.Info("AutoBackup: Deleting file: {0}, CreatedAt: {1}", oldestFile.Name, oldestFile.CreationTime);
                        // 删除最旧的文件
                        oldestFile.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// 手动备份
        /// </summary>
        public void ManualBackup()
        {
            ExecuteInternal();
            if(Settings.ResetIntervalAfterManualBackup)
            {
                _timer.IsEnabled = false;
                _timer.IsEnabled = Settings.IsEnabled;
            }
        }

    }
}