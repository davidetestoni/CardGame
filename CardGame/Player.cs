using CardGame.Models;
using System.Collections.Generic;
using System.Linq;

namespace CardGame
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
        public List<Card> Hand { get; set; }

        /// <summary>
        /// The cards in the deck.
        /// </summary>
        public List<Card> Deck { get; set; }

        /// <summary>
        /// The cards on the field.
        /// </summary>
        public List<CreatureCard> Field { get; set; }

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
