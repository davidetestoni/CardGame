using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Game
{
    public class GameEndedEventArgs : GameEventArgs
    {
        public PlayerInstance Winner { get; set; }
        public bool Surrendered { get; set; }
    }
}
