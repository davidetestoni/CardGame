using System;

namespace CardGame.Shared.Messages.Cards
{
    /// <summary>
    /// Message that relays information about damage taken.
    /// </summary>
    public class DamagedMessage : Message
    {
        public Guid TargetId { get; set; }
        public int Value { get; set; }
    }
}
