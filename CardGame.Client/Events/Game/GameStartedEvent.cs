using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Game
{
    public class GameStartedEvent : GameEvent
    {
        public PlayerInstance CurrentPlayer { get; set; }
    }
}
