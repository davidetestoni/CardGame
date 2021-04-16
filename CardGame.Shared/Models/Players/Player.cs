using CardGame.Shared.Models.Cards;
using System.Collections.Generic;

namespace CardGame.Shared.Models.Players
{
    /// <summary>
    /// This is the representation of a player, to be used during the
    /// creation of a <see cref="GameInstance"/>.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The deck of the player as a list of cards and quantity.
        /// </summary>
        public List<(Card, int)> Deck { get; set; }
    }
}
