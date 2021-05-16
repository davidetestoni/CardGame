using System.Collections.Generic;

namespace CardGame.Shared.Messages.Client.System
{
    public class JoinGameRequest : ClientMessage
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// The deck of the player, as shortnames and quantity.
        /// </summary>
        public Dictionary<string, int> Deck { get; set; }
    }
}
