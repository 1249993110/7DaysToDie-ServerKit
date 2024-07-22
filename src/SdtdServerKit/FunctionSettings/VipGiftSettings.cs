using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.FunctionSettings
{
    public class VipGiftSettings : ISettings
    {
        /// <summary>
        /// 是否启用功能
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 领取命令
        /// </summary>
        public string ClaimCmd { get; set; }

        /// <summary>
        /// 已经领取过提示
        /// </summary>
        public string HasClaimedTip { get; set; }

        /// <summary>
        /// 非VIP提示
        /// </summary>
        public string NonVipTip { get; set; }

        /// <summary>
        /// 领取成功提示
        /// </summary>
        public string ClaimSuccessTip { get; set; }
    }
}
