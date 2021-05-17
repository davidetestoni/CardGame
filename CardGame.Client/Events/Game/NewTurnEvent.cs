using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Game
{
    public class NewTurnEvent : GameEvent
    {
        public PlayerInstance CurrentPlayer { get; set; }
        public int TurnNumber { get; set; }
    }
}
