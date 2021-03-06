using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureAttackChangedEventArgs : GameEventArgs
    {
        public CreatureCardInstance Creature { get; set; }
        public int Amount { get; set; }
    }
}
