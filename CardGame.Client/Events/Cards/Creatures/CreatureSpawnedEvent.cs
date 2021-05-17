using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureSpawnedEvent : GameEvent
    {
        public CreatureCardInstance Creature { get; set; }
    }
}
