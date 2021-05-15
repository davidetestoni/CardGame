using System;

namespace CardGame.Shared.Messages.Server.Players
{
    public class PlayerManaRestoredMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player whose mana was restored.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The amount of mana restored.
        /// </summary>
        public int Amount { get; set; }
    }
}
