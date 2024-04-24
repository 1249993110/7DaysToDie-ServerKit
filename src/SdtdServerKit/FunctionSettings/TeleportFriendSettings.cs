namespace SdtdServerKit.FunctionSettings
{
    public class TeleportFriendSettings : ISettings
    {
        /// <summary>
        /// 是否启用功能
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 传送命令
        /// </summary>
        public string TeleCmdPrefix { get; set; }

        /// <summary>
        /// 传送间隔, 单位: 秒
        /// </summary>
        public int TeleInterval { get; set; }

        /// <summary>
        /// 需要积分
        /// </summary>
        public int PointsRequired { get; set; }

        /// <summary>
        /// 传送成功提示
        /// </summary>
        public string TeleSuccessTip { get; set; }

        /// <summary>
        /// 积分不足提示
        /// </summary>
        public string PointsNotEnoughTip { get; set; }

        /// <summary>
        /// 正在冷却提示
        /// </summary>
        public string CoolingTip { get; set; }

        /// <summary>
        /// 传送目标没有找到提示
        /// </summary>
        public string TargetNotFoundTip { get; set; }

        /// <summary>
        /// 传送目标不是您的好友提示
        /// </summary>
        public string TargetNotFriendTip { get; set; }
    }
}