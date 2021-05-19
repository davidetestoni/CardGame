using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Game
{
    public class NewTurnEventArgs : GameEventArgs
    {
        public PlayerInstance CurrentPlayer { get; set; }
        public int TurnNumber { get; set; }
    }
}
