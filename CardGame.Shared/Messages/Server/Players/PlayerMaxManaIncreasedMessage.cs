using System;

namespace CardGame.Shared.Messages.Server.Players
{
    public class PlayerMaxManaIncreasedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the player whose max mana got increased.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The increment of max mana.
        /// </summary>
        public int Increment { get; set; }
    }
}
