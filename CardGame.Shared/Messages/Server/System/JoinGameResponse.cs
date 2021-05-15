using System;

namespace CardGame.Shared.Messages.Server.System
{
    public class JoinGameResponse : ServerMessage
    {
        /// <summary>
        /// The id of the game that was joined.
        /// </summary>
        public Guid GameId { get; set; }

        /// <summary>
        /// The id of the player in the game.
        /// </summary>
        public Guid PlayerId { get; set; }
    }
}
