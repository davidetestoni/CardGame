using System;

namespace CardGame.Shared.DTOs
{
    public class GameInfoDTO
    {
        /// <summary>
        /// The id of the game.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The initial health of player.
        /// </summary>
        public int InitialHealth { get; set; }

        /// <summary>
        /// The maximum number of cards that can fit in a player's field.
        /// </summary>
        public int FieldSize { get; set; }
    }
}
