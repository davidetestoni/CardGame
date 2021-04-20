﻿using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Shared.Models.Cards;
using System;

namespace CardGame.Server.Models.Cards.Instances
{
    /// <summary>
    /// This is the instance of a card in the game. When a <see cref="Card"/> is
    /// instanced, it will become a <see cref="CardInstance"/>.
    /// </summary>
    public abstract class CardInstance
    {
        /// <summary>
        /// A reference to the game instance to which the card belongs.
        /// </summary>
        public GameInstance Game { get; set; }

        /// <summary>
        /// A reference to the player that currently owns this card.
        /// </summary>
        public PlayerInstance Owner { get; set; }

        /// <summary>
        /// The unique id of this card instance.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The current mana cost of the card instance.
        /// </summary>
        public int ManaCost { get; set; }
    }
}
