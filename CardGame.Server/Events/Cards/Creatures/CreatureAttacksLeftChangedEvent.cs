using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureAttacksLeftChangedEvent : GameEvent
    {
        public CreatureCardInstance Creature { get; set; }
        public bool CanAttack { get; set; }
    }
}
