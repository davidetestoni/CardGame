using CardGame.Models.Cards.Instances;
using System;

namespace CardGame.Models.Cards
{
    /// <summary>
    /// This is the representation of a game card with its base values.
    /// </summary>
    public abstract class Card
    {
        /// <summary>
        /// The unique id of the card, (e.g. Soldier).
        /// </summary>
        public string Id { get; set; }

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

        /// <summary>
        /// Creates an instance of the card.
        /// </summary>
        public virtual CardInstance CreateInstance(GameInstance game) => throw new NotImplementedException();
    }
}
