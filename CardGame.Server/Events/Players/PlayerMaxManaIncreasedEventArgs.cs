using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerMaxManaIncreasedEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public int Increment { get; set; }
    }
}
