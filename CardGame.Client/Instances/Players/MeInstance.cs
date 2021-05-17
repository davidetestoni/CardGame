using CardGame.Client.Instances.Cards;
using CardGame.Shared.Models.Cards;
using System.Collections.Generic;

namespace CardGame.Client.Instances.Players
{
    public class MeInstance : PlayerInstance
    {
        /// <summary>
        /// The cards in the hand.
        /// </summary>
        public List<CardInstance> Hand { get; set; } = new List<CardInstance>();

        /// <summary>
        /// The cards in the deck.
        /// </summary>
        public List<CardInstance> Deck { get; set; }
    }
}
