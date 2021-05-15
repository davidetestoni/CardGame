using System;

namespace CardGame.Shared.Messages.Server.Players
{
    public class PlayerDamagedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player that took damage.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The amount of damage taken.
        /// </summary>
        public int Damage { get; set; }
    }
}
