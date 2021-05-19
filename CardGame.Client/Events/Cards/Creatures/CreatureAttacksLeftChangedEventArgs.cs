using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureAttacksLeftChangedEventArgs : GameEventArgs
    {
        public CreatureCardInstance Creature { get; set; }
        public bool CanAttack { get; set; }
    }
}
