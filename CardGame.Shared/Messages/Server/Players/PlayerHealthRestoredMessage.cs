using System;

namespace CardGame.Shared.Messages.Server.Players
{
    public class PlayerHealthRestoredMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player whose health got restored.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The amount of health restored.
        /// </summary>
        public int Amount { get; set; }
    }
}
