using System;
using System.Collections.Generic;

namespace CardGame.Shared.Messages.Server.Game
{
    public class CardsDrawnMessage : ServerMessage
    {
        /// <summary>
        /// The cards that were drawn.
        /// </summary>
        public List<Guid> NewCards { get; set; }

        /// <summary>
        /// The new deck size after the draw.
        /// </summary>
        public int DeckSize { get; set; }
    }
}
