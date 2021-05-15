using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureAttackChangedEvent : GameEvent
    {
        public CreatureCardInstance Creature { get; set; }
        public int NewValue { get; set; }
    }
}
