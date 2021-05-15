using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerMaxManaIncreasedEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public int Increment { get; set; }
    }
}
