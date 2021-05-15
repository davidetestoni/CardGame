using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreatureDamagedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the creature that took damage.
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// The amount of damage caused.
        /// </summary>
        public int Damage { get; set; }
    }
}
