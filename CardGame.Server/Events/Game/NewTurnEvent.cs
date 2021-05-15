using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Game
{
    public class NewTurnEvent : GameEvent
    {
        public PlayerInstance CurrentPlayer { get; set; }
        public int TurnNumber { get; set; }
    }
}
