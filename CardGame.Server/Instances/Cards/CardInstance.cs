using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using System;

namespace CardGame.Server.Instances.Cards
{
    /// <summary>
    /// Server-side instance of a card.
    /// </summary>
    public abstract class CardInstance
    {
        /// <summary>
        /// A reference to the game instance to which the card belongs.
        /// </summary>
        public GameInstance Game { get; set; }

        /// <summary>
        /// A reference to the player that currently owns this card.
        /// </summary>
        public PlayerInstance Owner { get; set; }

        /// <summary>
        /// The unique id of this card instance.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The current mana cost of the card instance.
        /// </summary>
        public int ManaCost { get; set; }

        /// <summary>
        /// The shortname of the base card.
        /// </summary>
        public virtual string ShortName { get; }
    }
}
