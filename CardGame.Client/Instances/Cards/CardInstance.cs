using CardGame.Client.Instances.Players;
using CardGame.Shared.Models.Cards;
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

        /// <summary>
        /// The player that owns this card instance.
        /// </summary>
        public PlayerInstance Owner { get; set; }

        /// <summary>
        /// The base card from which this instance was created.
        /// </summary>
        public Card Base { get; set; }

        /// <summary>
        /// The shortname of this card.
        /// </summary>
        public string ShortName => Base.ShortName;
    }
}
