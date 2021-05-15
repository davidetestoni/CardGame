using CardGame.Shared.Enums;

namespace CardGame.Client.Instances.Cards
{
    public class CreatureCardInstance : CardInstance
    {
        /// <summary>
        /// The current attack this card has.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// The current health this card has.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Whether the card can attack this turn.
        /// </summary>
        public bool CanAttack { get; set; }

        /// <summary>
        /// Gets the current card features.
        /// </summary>
        public CardFeature Features { get; set; } = CardFeature.None;
    }
}
