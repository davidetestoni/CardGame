namespace CardGame.Client.Instances.Game
{
    /// <summary>
    /// Options of a <see cref="GameInstance"/>.
    /// </summary>
    public class GameInstanceOptions
    {
        /// <summary>
        /// The initial amount of health a player has.
        /// </summary>
        public int InitialHealth { get; set; } = 30;

        /// <summary>
        /// The maximum amount of creatures on the field for a player.
        /// </summary>
        public int FieldSize { get; set; } = 5;
    }
}
