using System.Collections.Generic;

namespace CardGame.Server.Models
{
    public class PlayerDefinition
    {
        public string Name { get; set; }
        public List<Card> Deck { get; set; }
    }
}
