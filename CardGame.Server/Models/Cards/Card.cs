namespace CardGame.Server.Models.Cards
{
    /// <summary>
    /// This is the representation of a game card with its base values.
    /// </summary>
    public abstract class Card
    {
        /// <summary>
        /// The unique shortname of the card, (e.g. Soldier).
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The name of the card, (e.g. Basic Soldier).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much mana is needed to summon this card.
        /// </summary>
        public int ManaCost { get; set; }

        /// <summary>
        /// The description of the card.
        /// </summary>
        public string Description { get; set; }
    }
}
