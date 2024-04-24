namespace SdtdServerKit.FunctionSettings
{
    public class TeleportHomeSettings : ISettings
    {
        /// <summary>
        /// 是否启用功能
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 查询Home列表命令
        /// </summary>
        public string QueryListCmd { get; set; } = "home";

        /// <summary>
        /// 传送间隔, 单位: 秒
        /// </summary>
        public int TeleInterval { get; set; } = 60;

        /// <summary>
        /// 设置Home命令前缀
        /// </summary>
        public string SetHomeCmdPrefix { get; set; } = "setHome";

        /// <summary>
        /// 最大可设置数量
        /// </summary>
        public int SetCountLimit { get; set; } = 3;

        /// <summary>
        /// 设置需要积分
        /// </summary>
        public int PointsRequiredForSet { get; set; } = 2;

        /// <summary>
        /// 删除Home命令前缀
        /// </summary>
        public string DeleteHomeCmdPrefix { get; set; } = "delHome";

        /// <summary>
        /// 传送Home命令前缀
        /// </summary>
        public string TeleHomeCmdPrefix { get; set; } = "home";

        /// <summary>
        /// 传送需要积分
        /// </summary>
        public int PointsRequiredForTele { get; set; } = 2;

        /// <summary>
        /// 没有Home提示
        /// </summary>
        public string NoHomeTip { get; set; } = "[00FF00]您尚未设置过家，请输入 /setHome 设置";

        /// <summary>
        /// 查询列表提示
        /// </summary>
        public string LocationItemTip { get; set; } = "[00FF00][00FF00]<[FF0000]{homeName}[00FF00]> 传送命令: [FF0000]/goHome {HomeName}, 需要积分: 2, 坐标: {Position}";

        /// <summary>
        /// 超出限制提示
        /// </summary>
        public string OverLimitTip { get; set; } = "[00FF00]超过最大设置数，您最多可设置3个家";

        /// <summary>
        /// 设置积分不足提示
        /// </summary>
        public string SetPointsNotEnoughTip { get; set; } = "[00FF00]积分不够! 需要积分: 2";

        /// <summary>
        /// 设置成功提示
        /// </summary>
        public string SetSuccessTip { get; set; } = "[00FF00]设置成功";

        /// <summary>
        /// 覆盖成功提示
        /// </summary>
        public string OverwriteSuccessTip { get; set; } = "[00FF00]已成功覆盖旧坐标";

        /// <summary>
        /// 删除成功提示
        /// </summary>
        public string DeleteSuccessTip { get; set; } = "[00FF00]删除成功";

        /// <summary>
        /// Home没有找到提示
        /// </summary>
        public string HomeNotFoundTip { get; set; } = "[00FF00]没有找到指定的家";

        /// <summary>
        /// 正在冷却提示
        /// </summary>
        public string CoolingTip { get; set; } = "[00FF00]传送冷却... 剩余时间: {CoolingTime} 秒";

        /// <summary>
        /// 传送积分不足提示
        /// </summary>
        public string TelePointsNotEnoughTip { get; set; } = "[00FF00]积分不够! 需要积分: 2";

        /// <summary>
        /// 传送成功提示
        /// </summary>
        public string TeleSuccessTip { get; set; } = "[00FF00]玩家: {PlayerName}, 传送到了自己的家: {HomeName}";
    }
}