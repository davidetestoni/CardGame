using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreaturePlayedOpponentMessage : ServerMessage
    {
        public Guid CreatureId { get; set; }
        public string ShortName { get; set; }
    }
}
