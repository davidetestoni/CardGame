using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreatureAttacksLeftChangedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the creature whose attacks left got changed.
        /// </summary>
        public Guid CreatureId { get; set; }

        /// <summary>
        /// Whether the creature can attack or not.
        /// </summary>
        public bool CanAttack { get; set; }
    }
}
