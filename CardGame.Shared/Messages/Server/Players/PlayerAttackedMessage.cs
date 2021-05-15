using System;

namespace CardGame.Shared.Messages.Server.Players
{
    public class PlayerAttackedMessage : ServerMessage
    {
        /// <summary>
        /// The id of player that was attacked.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The id of the creature that attacked the player.
        /// </summary>
        public Guid AttackerId { get; set; }

        /// <summary>
        /// The amount of damage taken.
        /// </summary>
        public int Damage { get; set; }
    }
}
