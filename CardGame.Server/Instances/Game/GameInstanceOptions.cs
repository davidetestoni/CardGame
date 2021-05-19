namespace CardGame.Server.Instances.Game
{
    /// <summary>
    /// Options of a <see cref="GameInstance"/>.
    /// </summary>
    public class GameInstanceOptions
    {
        /// <summary>
        /// The maximum amount of mana players can gain.
        /// </summary>
        public int MaximumMana { get; set; } = 10;

        /// <summary>
        /// The initial amount of health a player has.
        /// </summary>
        public int InitialHealth { get; set; } = 30;

        /// <summary>
        /// The maximum amount of cards players can have in their hand.
        /// </summary>
        public int MaximumHandSize { get; set; } = 10;

        /// <summary>
        /// The initial amount of cards that will be drawn.
        /// </summary>
        public int InitialHandSize { get; set; } = 5;

        /// <summary>
        /// The maximum amount of creatures on the field for a player.
        /// </summary>
        public int FieldSize { get; set; } = 5;
    }
}
