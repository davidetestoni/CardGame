using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerDamagedEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public int Damage { get; set; }
    }
}
