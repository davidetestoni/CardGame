using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Players
{
    public class PlayerManaSpentEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public int Amount { get; set; }
    }
}
