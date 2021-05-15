using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerManaRestoredEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public int Amount { get; set; }
    }
}
