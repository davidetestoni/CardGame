using System;

namespace CardGame.Shared.Messages.Server.Game
{
    public class CardsDrawnOpponentMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player that drew the cards.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// How many cards were drawn.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The new deck size after the draw.
        /// </summary>
        public int DeckSize { get; set; }
    }
}
