using CardGame.Shared.DTOs;
using System;
using System.Collections.Generic;

namespace CardGame.Shared.Messages.Server.Game
{
    public class GameStartedMessage : ServerMessage
    {
        /// <summary>
        /// Whether it's the player's turn.
        /// </summary>
        public bool MyTurn { get; set; }

        /// <summary>
        /// The info of the opponent.
        /// </summary>
        public StartGameInfoDTO GameInfo { get; set; }

        /// <summary>
        /// The cards in the deck (in random order).
        /// </summary>
        public List<CardInfoDTO> Deck { get; set; }
    }
}
