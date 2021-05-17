using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Game
{
    public class GameEndedEvent : GameEvent
    {
        public PlayerInstance Winner { get; set; }
        public bool Surrender { get; set; }
    }
}
