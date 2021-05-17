using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureAttacksLeftChangedEvent : GameEvent
    {
        public CreatureCardInstance Creature { get; set; }
        public bool CanAttack { get; set; }
    }
}
