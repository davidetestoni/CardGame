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
        /// The new value of the attack.
        /// </summary>
        public int NewValue { get; set; }
    }
}
