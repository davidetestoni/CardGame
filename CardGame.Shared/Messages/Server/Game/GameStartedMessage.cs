using System;

namespace CardGame.Shared.Messages.Server.Game
{
    public class GameStartedMessage : ServerMessage
    {
        /// <summary>
        /// The player that goes first.
        /// </summary>
        public Guid CurrentPlayerId { get; set; }
    }
}
