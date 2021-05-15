using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreatureDestroyedMessage : ServerMessage
    {
        public Guid CreatureId { get; set; }
    }
}
