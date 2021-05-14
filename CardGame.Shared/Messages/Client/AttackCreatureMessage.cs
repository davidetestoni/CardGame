using System;

namespace CardGame.Shared.Messages.Client
{
    /// <summary>
    /// Message that relays information about an attack.
    /// </summary>
    public class AttackCreatureMessage : ClientMessage
    {
        /// <summary>
        /// The id of the card that performed the attack.
        /// </summary>
        public Guid AttackerId { get; set; }

        /// <summary>
        /// The id of the card that received the attack.
        /// </summary>
        public Guid TargetId { get; set; }
    }
}
