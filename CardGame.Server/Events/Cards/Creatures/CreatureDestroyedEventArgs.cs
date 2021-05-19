using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureDestroyedEventArgs : GameEventArgs
    {
        public CardInstance Destroyer { get; set; }
        public CreatureCardInstance Target { get; set; }
    }
}
