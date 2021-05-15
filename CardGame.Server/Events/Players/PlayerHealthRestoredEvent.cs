using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerHealthRestoredEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public int Amount { get; set; }
    }
}
