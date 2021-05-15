using CardGame.Server.Instances.Cards;
using CardGame.Server.Instances.Players;

namespace CardGame.Server.Events.Players
{
    public class PlayerAttackedEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public CreatureCardInstance Attacker { get; set; }
        public int Damage { get; set; }
    }
}
