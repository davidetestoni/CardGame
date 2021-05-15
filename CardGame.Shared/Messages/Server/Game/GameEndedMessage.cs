using System;

namespace CardGame.Shared.Messages.Server.Game
{
    public class GameEndedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the winning player.
        /// </summary>
        public Guid WinnerId { get; set; }

        /// <summary>
        /// Whether the game ended due to a surrender.
        /// </summary>
        public bool Surrender { get; set; }
    }
}
