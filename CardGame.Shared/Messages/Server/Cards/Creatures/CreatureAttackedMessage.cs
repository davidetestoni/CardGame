using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreatureAttackedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the creature that performed the attack.
        /// </summary>
        public Guid AttackerId { get; set; }

        /// <summary>
        /// The id of the creature that was attacked.
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// The damage inflicted to the target.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// The damage taken by the attacker.
        /// </summary>
        public int RecoilDamage { get; set; }
    }
}
