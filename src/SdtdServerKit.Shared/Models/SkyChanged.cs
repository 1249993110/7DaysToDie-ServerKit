namespace SdtdServerKit.Shared.Models
{
    public class SkyChanged
    {
        public SkyChangeEventType SkyChangeEventType { get; set; }

        /// <summary>
        /// 黎明时刻
        /// </summary>
        public int DawnHour { get; set; }

        /// <summary>
        /// 黄昏时刻
        /// </summary>
        public int DuskHour { get; set; }

        /// <summary>
        /// 血月剩余天数
        /// </summary>
        public int BloodMoonDaysRemaining { get; set; }
    }
}