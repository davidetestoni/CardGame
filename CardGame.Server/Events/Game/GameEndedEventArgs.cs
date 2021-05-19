using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Game
{
    public class GameEndedEventArgs : GameEventArgs
    {
        public PlayerInstance Winner { get; set; }
        public bool Surrendered { get; set; }
    }
}
