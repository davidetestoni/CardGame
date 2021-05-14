using CardGame.Shared.Models.Effects;
using System.Collections.Generic;

namespace CardGame.Shared.Models.Cards
{
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

        /// <summary>
        /// The effects of this card.
        /// </summary>
        public List<Effect> Effects { get; set; } = new List<Effect>();
    }
}
