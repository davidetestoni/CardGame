using System.Collections.Generic;

namespace CardGame.Server.Models.Cards
{
    public class CreatureCard : Card
    {
        /// <summary>
        /// The base value of the attack of this card.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// The base value of the health of this card.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// The effects of this card.
        /// </summary>
        public List<Effect> Effects { get; set; }
    }
}
