using CardGame.Server.Instances.Cards;
using CardGame.Server.Instances.Players;
using System.Collections.Generic;

namespace CardGame.Server.Events.Game
{
    public class CardsDrawnEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public List<CardInstance> NewCards { get; set; }
        public List<CardInstance> Destroyed { get; set; }
    }
}
