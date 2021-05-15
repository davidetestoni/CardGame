using System;

namespace CardGame.Client.Instances.Cards
{
    public class CardInstance
    {
        /// <summary>
        /// The unique id of this card instance.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The current mana cost of the card instance.
        /// </summary>
        public int ManaCost { get; set; }
    }
}
