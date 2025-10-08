namespace SdtdServerKit.FunctionSettings
{
    public class Trigger
    {
        /// <summary>
        /// Gets or sets a value indicating whether the trigger is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the commands to execute when the trigger is activated.
        /// </summary>
        public string[] ExecuteCommands { get; set; }
    }
    public class AutoRestart
    {
        /// <summary>
        /// Gets or sets a value indicating whether auto restart is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the hour at which the server should restart.
        /// </summary>
        public int RestartHour { get; set; }

        /// <summary>
        /// Gets or sets the minute at which the server should restart.
        /// </summary>
        public int RestartMinute { get; set; }

        /// <summary>
        /// Gets or sets the messages to display before the server restarts.
        /// </summary>
        public string[] Messages { get; set; }
    }

    /// <summary>
    /// Global settings.
    /// </summary>
    public class GlobalSettings : SettingsBase
    {
        /// <summary>
        /// Gets or sets the global server name.
        /// </summary>
        public string GlobalServerName { get; set; }

        /// <summary>
        /// Gets or sets the whisper server name.
        /// </summary>
        public string WhisperServerName { get; set; }

        /// <summary>
        /// Gets or sets the chat command prefix.
        /// </summary>
        public string ChatCommandPrefix { get; set; }

        /// <summary>
        /// Gets or sets the chat command separator.
        /// </summary>
        public string ChatCommandSeparator { get; set; }

        /// <summary>
        /// Gets or sets the error message for handling chat messages.
        /// </summary>
        public string HandleChatMessageError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to check for zombies around the player before teleporting.
        /// </summary>
        public bool TeleZombieCheck { get; set; }

        /// <summary>
        /// Gets or sets the disable teleportation tip.
        /// </summary>
        public string TeleDisableTip { get; set; }

        /// <summary>
        /// Gets or sets the trigger for killing zombies.
        /// </summary>
        public Trigger KillZombieTrigger { get; set; }

        /// <summary>
        /// Gets or sets the trigger for player death.
        /// </summary>
        public Trigger DeathTrigger { get; set; }

        /// <summary>
        /// Gets or sets the auto restart settings.
        /// </summary>
        public AutoRestart AutoRestart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to block family sharing accounts from joining the server.
        /// </summary>
        public bool BlockFamilySharingAccount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remove sleeping bags from Points of Interest (POI).
        /// </summary>
        public bool RemoveSleepingBagFromPOI { get; set; }

        public bool EnableLandClaimProtection { get; set; }
        public string? LandClaimProtectionTip { get; set; }
        public bool EnableTraderAreaProtection { get; set; }
        public string? TraderAreaProtectionTip { get; set; }

        /// <summary>
        /// Gets or sets the tip for removing sleeping bags from Points of Interest (POI).
        /// </summary>
        public string? RemoveSleepingBagFromPoiTip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable player initial spawn point.
        /// </summary>
        public bool IsEnablePlayerInitialSpawnPoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableFallingBlockProtection { get; set; }

        /// <summary>
        /// Gets or sets the player's initial position.
        /// </summary>
        public string PlayerInitialPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable automatic zombie cleanup.
        /// </summary>
        public bool EnableAutoZombieCleanup { get; set; }

        /// <summary>
        /// Gets or sets the auto zombie cleanup threshold.
        /// </summary>
        public int AutoZombieCleanupThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable XMLs secondary overwrites.
        /// </summary>
        public bool EnableXmlsSecondaryOverwrite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide the command in chat.
        /// </summary>
        public bool HideCommandInChat { get; set; }
    }
}