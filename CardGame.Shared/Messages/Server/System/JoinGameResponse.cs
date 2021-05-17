using CardGame.Shared.DTOs;
using System;

namespace CardGame.Shared.Messages.Server.System
{
    public class JoinGameResponse : ServerMessage
    {
        /// <summary>
        /// The id of the player in the game.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// Information about the current game.
        /// </summary>
        public GameInfoDTO GameInfo { get; set; }
    }
}
