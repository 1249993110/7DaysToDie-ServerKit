using System.Collections.Generic;

namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 统计
    /// </summary>
    public class Stats
    {
        /// <summary>
        /// 服务端正常运行时间
        /// </summary>
        public float Uptime { get; set; }

        /// <summary>
        /// 游戏时间
        /// </summary>
        public GameTime GameTime { get; set; } = null!;

        /// <summary>
        /// 动物数
        /// </summary>
        public int Animals { get; set; }

        /// <summary>
        /// 僵尸数
        /// </summary>
        public int Zombies { get; set; }

        /// <summary>
        /// 实体数
        /// </summary>
        public int Entities { get; set; }

        /// <summary>
        /// 在线玩家数
        /// </summary>
        public int OnlinePlayers { get; set; }

        /// <summary>
        /// 离线玩家数量
        /// </summary>
        public int OfflinePlayers { get; set; }

        /// <summary>
        /// 是否为血月
        /// </summary>
        public bool IsBloodMoon { get; set; }

        /// <summary>
        /// Frames Per Second
        /// </summary>
        [JsonProperty("fps")]
        public float FPS { get; set; }

        /// <summary>
        /// 堆内存使用量, 以兆字节 (MB) 为单位, 表示当前游戏所使用的堆内存大小
        /// </summary>
        public float Heap { get; set; }

        /// <summary>
        /// 堆内存的最大限制，以兆字节 (MB) 为单位，表示堆内存的最大可用容量
        /// </summary>
        public float MaxHeap { get; set; }

        /// <summary>
        /// 游戏中的区块数量
        /// </summary>
        public int Chunks { get; set; }

        /// <summary>
        /// 指当前游戏中的活跃对象数量
        /// </summary>
        [JsonProperty("cgo")]
        public int CGO { get; set; }

        /// <summary>
        /// 物品数
        /// </summary>
        public int Items { get; set; }

        /// <summary>
        /// CO
        /// </summary>
        public int ChunkObservedEntities { get; set; }

        /// <summary>
        /// 常驻内存大小, 表示当前游戏所占用的物理内存大小
        /// </summary>
        public float ResidentSetSize { get; set; }
    }
}