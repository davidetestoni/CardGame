using CardGame.Server.Models.Cards.Instances;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureSpawnedEvent : GameEvent
    {
        public CreatureCardInstance Creature { get; set; }
    }
}
