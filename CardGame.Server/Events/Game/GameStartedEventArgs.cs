using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Game
{
    public class GameStartedEventArgs : GameEventArgs
    {
        public PlayerInstance CurrentPlayer { get; set; }
    }
}
