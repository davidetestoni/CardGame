using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreaturePlayedMessage : ServerMessage
    {
        public Guid CreatureId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
