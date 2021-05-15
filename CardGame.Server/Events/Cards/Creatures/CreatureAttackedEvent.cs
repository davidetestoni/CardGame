using CardGame.Server.Models.Cards.Instances;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureAttackedEvent : GameEvent
    {
        public CardInstance Attacker { get; set; }
        public CardInstance Defender { get; set; }
        public int Damage { get; set; }
        public int RecoilDamage { get; set; }
    }
}
