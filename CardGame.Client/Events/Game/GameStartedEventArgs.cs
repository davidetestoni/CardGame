using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Game
{
    public class GameStartedEventArgs : GameEventArgs
    {
        public PlayerInstance CurrentPlayer { get; set; }
    }
}
