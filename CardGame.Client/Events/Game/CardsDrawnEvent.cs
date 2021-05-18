using CardGame.Client.Instances.Cards;
using System.Collections.Generic;

namespace CardGame.Client.Events.Game
{
    public class CardsDrawnEvent : GameEvent
    {
        public List<CardInstance> NewCards { get; set; }
        public List<CardInstance> Destroyed { get; set; }
    }
}
