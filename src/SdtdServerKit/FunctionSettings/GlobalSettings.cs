namespace SdtdServerKit.FunctionSettings
{
    public class Trigger
    {
        public bool IsEnabled { get; set; }
        public string[] ExecuteCommands { get; set; }
    }
    public class AutoRestart
    {
        public bool IsEnabled { get; set; }
        public int RestartHour { get; set; }
        public int RestartMinute { get; set; }
        public string[] Messages { get; set; }
    }

    /// <summary>
    /// 全局设置
    /// </summary>
    public class GlobalSettings : ISettings
    {
        /// <summary>
        /// 全局服务器名称
        /// </summary>
        public string GlobalServerName { get; set; }

        /// <summary>
        /// 私聊服务器名称
        /// </summary>
        public string WhisperServerName { get; set; }

        /// <summary>
        /// 聊天命令前缀
        /// </summary>
        public string ChatCommandPrefix { get; set; }

        /// <summary>
        /// 聊天命令分隔符
        /// </summary>
        public string ChatCommandSeparator { get; set; }

        /// <summary>
        /// 处理聊天消息错误提示
        /// </summary>
        public string HandleChatMessageError { get; set; }

        bool ISettings.IsEnabled { get; set; }

        /// <summary>
        /// 传送前是否检查玩家周围是否有僵尸
        /// </summary>
        public bool TeleZombieCheck { get; set; }

        /// <summary>
        /// 禁止传送提示
        /// </summary>
        public string TeleDisableTip { get; set; }

        public Trigger KillZombieTrigger { get; set; }
        public Trigger DeathTrigger { get; set; }
        public AutoRestart AutoRestart { get; set; }

        /// <summary>
        /// 禁止家庭共享账号加入服务器
        /// </summary>
        public bool BlockFamilySharingAccount { get; set; }

        /// <summary>
        /// 是否移除POI中的睡袋
        /// </summary>
        public bool RemoveSleepingBagFromPOI { get; set; }

        /// <summary>
        /// 是否启用玩家初始出生点
        /// </summary>
        public bool IsEnablePlayerInitialSpawnPoint { get; set; }

        /// <summary>
        /// 玩家初始坐标
        /// </summary>
        public string PlayerInitialPosition { get; set; }
    }
}