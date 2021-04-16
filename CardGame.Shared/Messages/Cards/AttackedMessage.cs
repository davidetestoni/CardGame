using System;

namespace CardGame.Shared.Messages.Cards
{
    /// <summary>
    /// Message that relays information about an attack.
    /// </summary>
    public class AttackedMessage : Message
    {
        public Guid AttackerId { get; set; }
        public Guid TargetId { get; set; }
    }
}
