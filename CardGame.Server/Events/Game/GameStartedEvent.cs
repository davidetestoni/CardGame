using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Game
{
    public class GameStartedEvent : GameEvent
    {
        public PlayerInstance CurrentPlayer { get; set; }
    }
}
