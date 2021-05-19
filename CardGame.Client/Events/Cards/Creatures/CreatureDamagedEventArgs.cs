using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureDamagedEventArgs : GameEventArgs
    {
        public CardInstance Target { get; set; }
        public int Damage { get; set; }
    }
}
