using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureDestroyedEventArgs : GameEventArgs
    {
        public CardInstance Destroyer { get; set; }
        public CreatureCardInstance Target { get; set; }
    }
}
