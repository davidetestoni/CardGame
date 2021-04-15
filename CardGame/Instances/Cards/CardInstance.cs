namespace CardGame.Models.Cards.Instances
{
    /// <summary>
    /// This is the instance of a card in the game. When a <see cref="Card"/> is
    /// instanced, it will become a <see cref="CardInstance"/>.
    /// </summary>
    public abstract class CardInstance
    {
        /// <summary>
        /// A reference to the game instance to which the card belongs.
        /// </summary>
        public GameInstance Game { get; set; }

        /// <summary>
        /// This is the original card from which the instance was created.
        /// </summary>
        public Card Base { get; set; }

        /// <summary>
        /// The current mana cost of the card instance.
        /// </summary>
        public int ManaCost { get; set; }
    }
}
