namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 生成类型
    /// </summary>
    public enum RespawnType : byte
    {
        /// <summary>
        ///
        /// </summary>
        NewGame = 0,

        /// <summary>
        ///
        /// </summary>
        LoadedGame = 1,

        /// <summary>
        /// 死亡
        /// </summary>
        Died = 2,

        /// <summary>
        /// 传送
        /// </summary>
        Teleport = 3,

        /// <summary>
        /// 新玩家进入多人游戏
        /// </summary>
        EnterMultiplayer = 4,

        /// <summary>
        /// 旧玩家加入多人游戏
        /// </summary>
        JoinMultiplayer = 5,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 6
    }
}