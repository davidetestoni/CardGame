using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Game
{
    public class NewTurnEventArgs : GameEventArgs
    {
        public PlayerInstance CurrentPlayer { get; set; }
        public int TurnNumber { get; set; }
    }
}
