using CardGame.Shared.Models.Effects;
using System.Collections.Generic;

namespace CardGame.Shared.Models.Cards
{
    public class CreatureCard : Card
    {
        /// <summary>
        /// The base value of the attack of this card.
        /// </summary>
        public virtual int Attack { get; } = 1;

        /// <summary>
        /// The base value of the health of this card.
        /// </summary>
        public virtual int Health { get; } = 1;

        /// <summary>
        /// The effects of this card.
        /// </summary>
        public virtual List<Effect> Effects { get; } = new();
    }
}
