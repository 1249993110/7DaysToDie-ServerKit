﻿namespace SdtdServerKit.Models
{
    /// <summary>
    /// 礼品
    /// </summary>
    public class VipGift
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 礼品名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 领取状态
        /// </summary>
        public bool ClaimState { get; set; }

        /// <summary>
        /// 总领取次数
        /// </summary>
        public int TotalClaimCount { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string? Description { get; set; }
    }
}
