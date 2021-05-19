using CardGame.Client.Instances.Cards;
using System.Collections.Generic;

namespace CardGame.Client.Events.Game
{
    public class CardsDrawnOpponentEventArgs : GameEventArgs
    {
        public int Amount { get; set; }
        public List<CardInstance> Destroyed { get; set; }
    }
}
