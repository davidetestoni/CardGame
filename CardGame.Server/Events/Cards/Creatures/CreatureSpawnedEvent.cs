using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureSpawnedEvent : GameEvent
    {
        public CreatureCardInstance Creature { get; set; }
    }
}
