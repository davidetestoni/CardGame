using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreatureHealthIncreasedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the creature whose health got increased.
        /// </summary>
        public Guid CreatureId { get; set; }

        /// <summary>
        /// The amount of health received.
        /// </summary>
        public int Amount { get; set; }
    }
}
