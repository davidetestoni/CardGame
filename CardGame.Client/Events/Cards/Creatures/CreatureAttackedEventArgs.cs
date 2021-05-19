using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureAttackedEventArgs : GameEventArgs
    {
        public CardInstance Attacker { get; set; }
        public CardInstance Defender { get; set; }
        public int Damage { get; set; }
        public int RecoilDamage { get; set; }
    }
}
