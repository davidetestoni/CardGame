namespace CardGame.Models
{
    public abstract class Card
    {
        /// <summary>
        /// The unique id of the card, for example Soldier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the card, for example Basic Soldier.
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
