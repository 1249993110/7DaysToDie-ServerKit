namespace SdtdServerKit.Models
{
    /// <summary>
    /// Represents the progression of a player.
    /// </summary>
    public class PlayerProgression
    {
        /// <summary>
        /// Gets or sets the level of the player.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the experience required to reach the next level.
        /// </summary>
        public int ExpToNextLevel { get; set; }

        /// <summary>
        /// Gets or sets the skill points available for the player.
        /// </summary>
        public int SkillPoints { get; set; }
    }
}
