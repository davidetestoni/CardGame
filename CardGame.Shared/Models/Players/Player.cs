﻿using System;
using System.Collections.Generic;

namespace CardGame.Shared.Models.Players
{
    /// <summary>
    /// This is the representation of a player, to be used during the
    /// creation of a <see cref="GameInstance"/>.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The id of the player. Randomly generated by default.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The deck of the player as a dictionary of shortnames and quantity.
        /// </summary>
        public Dictionary<string, int> Deck { get; set; }
    }
}
