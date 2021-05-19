using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Players
{
    public class PlayerDamagedEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public int Damage { get; set; }
    }
}
