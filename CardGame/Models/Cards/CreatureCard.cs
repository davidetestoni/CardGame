using CardGame.Models.Cards.Instances;
using System;
using System.Collections.Generic;

namespace CardGame.Models.Cards
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

        /// <inheritdoc/>
        public override CardInstance CreateInstance(GameInstance game)
        {
            return new CreatureCardInstance
            {
                Attack = Attack,
                Health = Health,
                ManaCost = ManaCost,
                Base = this,
                Game = game
            };
        }
    }
}
