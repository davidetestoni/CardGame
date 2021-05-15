using System;

namespace CardGame.Shared.Messages.Server.Game
{
    public class NewTurnMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player that is playing the new turn.
        /// </summary>
        public Guid CurrentPlayerId { get; set; }

        /// <summary>
        /// The number of turns since the start of the game.
        /// </summary>
        public int TurnNumber { get; set; }
    }
}
