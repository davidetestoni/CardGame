using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;
using System.Collections.Generic;

namespace CardGame.Server.Events.Game
{
    public class CardsDrawnEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public List<CardInstance> NewCards { get; set; }
    }
}
