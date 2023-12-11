namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 命令权限条目
    /// </summary>
    public class PermissionEntry
    {
        /// <summary>
        /// 命令
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 权限等级
        /// </summary>
        public int Level { get; set; }
    }
}