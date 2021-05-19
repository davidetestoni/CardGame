using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Players
{
    public class PlayerMaxManaIncreasedEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public int Increment { get; set; }
    }
}
