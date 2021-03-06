using System.Collections.Generic;

namespace CardGame.Models
{
    public class PlayerDefinition
    {
        public string Name { get; set; }
        public List<Card> Deck { get; set; }
    }
}
