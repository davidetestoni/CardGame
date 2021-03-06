using CardGame.Server.Instances.Cards;
using System;
using System.Collections.Generic;

namespace CardGame.Server.Instances.Players
{
    /// <summary>
    /// Represents a player of a <see cref="Game.GameInstance"/>.
    /// </summary>
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
        public List<CardInstance> Hand { get; set; } = new List<CardInstance>();

        /// <summary>
        /// The cards in the deck.
        /// </summary>
        public List<CardInstance> Deck { get; set; }

        /// <summary>
        /// The cards in the graveyard.
        /// </summary>
        public List<CardInstance> Graveyard { get; set; } = new List<CardInstance>();

        /// <summary>
        /// The cards on the field.
        /// </summary>
        public List<CreatureCardInstance> Field { get; set; } = new List<CreatureCardInstance>();

        /// <summary>
        /// The current amount of mana left for this turn.
        /// </summary>
        public int CurrentMana { get; set; } = 0;

        /// <summary>
        /// The maximum amount of mana for this turn.
        /// </summary>
        public int MaximumMana { get; set; } = 0;

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
