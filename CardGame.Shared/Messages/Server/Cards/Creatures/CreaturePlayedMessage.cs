using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreaturePlayedMessage : ServerMessage
    {
        public Guid PlayerId { get; set; }
        public Guid CreatureId { get; set; }
    }
}
