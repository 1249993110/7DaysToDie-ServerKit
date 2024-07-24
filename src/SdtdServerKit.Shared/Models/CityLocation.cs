﻿namespace SdtdServerKit.Shared.Models
{
    public class CityLocation
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 传送需要积分
        /// </summary>
        public int PointsRequired { get; set; }

        /// <summary>
        /// 三维坐标
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 视角方向
        /// </summary>
        public string? ViewDirection { get; set; }
    }
}
