namespace SdtdServerKit.FunctionSettings
{
    public class PointsSystemSettings : ISettings
    {
        /// <summary>
        /// 是否启用功能
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 签到命令
        /// </summary>
        public string SignInCmd { get; set; }

        /// <summary>
        /// 签到间隔秒数
        /// </summary>
        public int SignInInterval { get; set; }

        /// <summary>
        /// 签到奖励积分
        /// </summary>
        public int SignInRewardPoints { get; set; }

        /// <summary>
        /// 签到成功提示
        /// </summary>
        public string SignInSuccessTip { get; set; }

        /// <summary>
        /// 签到失败提示
        /// </summary>
        public string SignInFailureTip { get; set; }

        /// <summary>
        /// 查询积分命令
        /// </summary>
        public string QueryPointsCmd { get; set; }

        /// <summary>
        /// 查询积分提示
        /// </summary>
        public string QueryPointsTip { get; set; }

    }
}
