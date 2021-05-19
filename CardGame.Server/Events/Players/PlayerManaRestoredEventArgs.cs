using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerManaRestoredEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public int Amount { get; set; }
    }
}
