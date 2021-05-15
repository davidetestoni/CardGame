using System;

namespace CardGame.Shared.Messages.Server.Players
{
    public class PlayerManaSpentMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player whose mana was spent.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The amount of mana spent.
        /// </summary>
        public int Amount { get; set; }
    }
}
