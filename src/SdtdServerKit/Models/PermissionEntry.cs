namespace SdtdServerKit.Models
{
    /// <summary>
    /// Permission Entry Add
    /// </summary>
    public class PermissionEntryAdd
    {
        /// <summary>
        /// Command
        /// </summary>
        public string Command { get; set; } = null!;

        /// <summary>
        /// Permission Level
        /// </summary>
        public int PermissionLevel { get; set; }
    }

    /// <summary>
    /// Permission Entry
    /// </summary>
    public class PermissionEntry : PermissionEntryAdd
    {
        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; internal set; }
    }
}