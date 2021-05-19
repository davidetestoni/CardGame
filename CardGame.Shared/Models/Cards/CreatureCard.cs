namespace CardGame.Shared.Models.Cards
{
    /// <summary>
    /// This is the representation of a creature card with its base values.
    /// </summary>
    public class CreatureCard : Card
    {
        /// <summary>
        /// The base value of the attack of this card.
        /// </summary>
        public int Attack { get; set; } = 1;

        /// <summary>
        /// The base value of the health of this card.
        /// </summary>
        public int Health { get; set; } = 1;
    }
}
