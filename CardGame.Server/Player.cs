using CardGame.Server.Models.Cards;
using CardGame.Server.Models.Cards.Instances;
using System.Collections.Generic;

namespace CardGame.Server
{
    public class Player
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cards in the hand.
        /// </summary>
        public List<CardInstance> Hand { get; set; }

        /// <summary>
        /// The cards in the deck.
        /// </summary>
        public List<CardInstance> Deck { get; set; }

        /// <summary>
        /// The cards on the field.
        /// </summary>
        public List<CreatureCardInstance> Field { get; set; }

        /// <summary>
        /// The cards in the graveyard.
        /// </summary>
        public List<Card> Graveyard { get; set; }

        /// <summary>
        /// The current amount of mana left for this turn.
        /// </summary>
        public int CurrentMana { get; set; }

        /// <summary>
        /// The maximum amount of mana for this turn.
        /// </summary>
        public int MaximumMana { get; set; }
    }
}
