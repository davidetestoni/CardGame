using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Game
{
    public class GameEndedEvent : GameEvent
    {
        public PlayerInstance Winner { get; set; }
        public bool Surrender { get; set; }
    }
}
