using System;
using System.Collections.Generic;

namespace CardGame.Shared.Messages.Server.Game
{
    public class CardsDrawnMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player that drew the cards.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The cards that were drawn.
        /// </summary>
        public List<DrawnCardDTO> NewCards { get; set; }
    }
}
