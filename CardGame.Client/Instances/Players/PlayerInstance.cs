using CardGame.Client.Instances.Cards;
using CardGame.Shared.Models.Cards;
using System;
using System.Collections.Generic;

namespace CardGame.Client.Instances.Players
{
    public class PlayerInstance
    {
        public Guid Id { get; set; }

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
        public List<Card> Deck { get; set; }

        /// <summary>
        /// The cards in the graveyard.
        /// </summary>
        public List<Card> Graveyard { get; set; }

        /// <summary>
        /// The cards on the field.
        /// </summary>
        public List<CreatureCardInstance> Field { get; set; }

        /// <summary>
        /// The current amount of mana left for this turn.
        /// </summary>
        public int CurrentMana { get; set; }

        /// <summary>
        /// The maximum amount of mana for this turn.
        /// </summary>
        public int MaximumMana { get; set; }

        /// <summary>
        /// The current amount of health of the player.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// The initial amount of health the player has.
        /// </summary>
        public int InitialHealth { get; set; }
    }
}
