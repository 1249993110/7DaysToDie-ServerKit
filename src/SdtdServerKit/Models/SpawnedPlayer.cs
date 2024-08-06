namespace SdtdServerKit.Models
{
    /// <summary>
    /// Represents a spawned player.
    /// </summary>
    public class SpawnedPlayer : IPlayerBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlayerId { get; set; } = null!;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlayerName { get; set; } = null!;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PlatformId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Gets or sets the respawn type.
        /// </summary>
        public RespawnType RespawnType { get; set; }
    }
}