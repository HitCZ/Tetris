namespace Tetris
{
    /// <summary>
    /// Represents information about the number of rotations.
    /// </summary>
    public enum RotationState {
        /// <summary>
        /// Vychozi stav rotace
        /// </summary>
        Default,

        /// <summary>
        /// Objekt byl otocen jednou
        /// </summary>
        RotatedOnce,

        /// <summary>
        /// Objekt byl otocen dvakrat
        /// </summary>
        RotatedTwice,

        /// <summary>
        /// Objekt byl otocen trikrat
        /// </summary>
        RotatedThreeTimes
    }
}
