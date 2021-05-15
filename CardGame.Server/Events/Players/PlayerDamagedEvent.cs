using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerDamagedEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public int Damage { get; set; }
    }
}
