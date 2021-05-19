using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureDamagedEventArgs : GameEventArgs
    {
        public CardInstance Target { get; set; }
        public int Damage { get; set; }
    }
}
