using System;

namespace CardGame.Shared.Messages.Client.Cards.Creatures
{
    public class PlayCardMessage : ClientMessage
    {
        /// <summary>
        /// The id of the card played from the hand.
        /// </summary>
        public Guid CardId { get; set; }
    }
}
