namespace CardGame.Shared.Models.Cards
{
    /// <summary>
    /// This is the representation of a game card with its base values.
    /// </summary>
    public abstract class Card
    {
        /// <summary>
        /// The unique shortname of the card, (e.g. Soldier).
        /// </summary>
        public virtual string ShortName { get; } = "ShortName";

        /// <summary>
        /// The name of the card, (e.g. Basic Soldier).
        /// </summary>
        public virtual string Name { get; } = "Name";

        /// <summary>
        /// How much mana is needed to summon this card.
        /// </summary>
        public virtual int ManaCost { get; } = 1;

        /// <summary>
        /// The description of the card.
        /// </summary>
        public virtual string Description { get; } = "Description";
    }
}
