using System;

namespace CardGame.Shared.DTOs
{
    public class OpponentInfoDTO
    {
        /// <summary>
        /// The id of the opponent.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of cards in the player's deck.
        /// </summary>
        public int DeckSize { get; set; }
    }
}
