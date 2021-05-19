using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreatureAttackChangedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the creature whose attack was changed.
        /// </summary>
        public Guid CreatureId { get; set; }

        /// <summary>
        /// The increase or decrease amount of the attack.
        /// </summary>
        public int Amount { get; set; }
    }
}
