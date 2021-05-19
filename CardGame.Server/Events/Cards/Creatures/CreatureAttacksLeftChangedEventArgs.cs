using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureAttacksLeftChangedEventArgs : GameEventArgs
    {
        public CreatureCardInstance Creature { get; set; }
        public bool CanAttack { get; set; }
    }
}
