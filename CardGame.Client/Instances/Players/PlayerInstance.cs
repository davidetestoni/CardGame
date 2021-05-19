using CardGame.Client.Instances.Cards;
using System;
using System.Collections.Generic;

namespace CardGame.Client.Instances.Players
{
    /// <summary>
    /// Represents a player locally.
    /// </summary>
    public abstract class PlayerInstance
    {
        /// <summary>
        /// The id of the player.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cards in the player's graveyard.
        /// </summary>
        public List<CardInstance> Graveyard { get; set; } = new List<CardInstance>();

        /// <summary>
        /// The cards on the player's field.
        /// </summary>
        public List<CreatureCardInstance> Field { get; set; } = new List<CreatureCardInstance>();

        /// <summary>
        /// The current amount of mana left for this player's turn.
        /// </summary>
        public int CurrentMana { get; set; } = 0;

        /// <summary>
        /// The maximum amount of mana for this player's turn.
        /// </summary>
        public int MaximumMana { get; set; } = 0;

        /// <summary>
        /// The current amount of health of the player.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// The initial amount of health the player has at the start of the game.
        /// </summary>
        public int InitialHealth { get; set; }
    }
}
