using CardGame.Shared.DTOs;
using System.Collections.Generic;

namespace CardGame.Shared.Messages.Server.Game
{
    public class CardsDrawnOpponentMessage : ServerMessage
    {
        /// <summary>
        /// How many cards were drawn.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The cards that were drawn and immediately destroyed because hand was full.
        /// </summary>
        public List<CardInfoDTO> Destroyed { get; set; }

        /// <summary>
        /// The new deck size after the draw.
        /// </summary>
        public int DeckSize { get; set; }
    }
}
