using System;

namespace CardGame.Shared.Messages.Client.Cards.Creatures
{
    public class AttackPlayerMessage : ClientMessage
    {
        /// <summary>
        /// The id of the card that performed the attack.
        /// </summary>
        public Guid AttackerId { get; set; }
    }
}
