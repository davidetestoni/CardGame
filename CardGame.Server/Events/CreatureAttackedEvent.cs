using CardGame.Server.Models.Cards.Instances;

namespace CardGame.Server.Events
{
    public class CreatureAttackedEvent : GameEvent
    {
        public CardInstance Attacker { get; set; }
        public CardInstance Target { get; set; }
    }
}
