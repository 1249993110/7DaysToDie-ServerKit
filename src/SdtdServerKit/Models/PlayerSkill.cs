namespace SdtdServerKit.Models
{
    /// <summary>
    /// Represents the skill of a player.
    /// </summary>
    public class PlayerSkill
    {
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the localized name of the player.
        /// </summary>
        public string? LocalizationName { get; set; }

        /// <summary>
        /// Gets or sets the localized description of the player.
        /// </summary>
        public string? LocalizationDesc { get; set; }

        ///// <summary>
        ///// Gets or sets the localized long description of the player.
        ///// </summary>
        //public string? LocalizationLongDesc { get; set; }

        /// <summary>
        /// Gets or sets the level of the player.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the minimum level of the player.
        /// </summary>
        public int MinLevel { get; set; }

        /// <summary>
        /// Gets or sets the maximum level of the player.
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Gets or sets the cost for the next level of the player.
        /// </summary>
        public int CostForNextLevel { get; set; }

        /// <summary>
        /// Gets or sets the icon of the player.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Gets or sets the children progressions of the player.
        /// </summary>
        public List<PlayerSkill>? Children { get; set; }
    }
}
