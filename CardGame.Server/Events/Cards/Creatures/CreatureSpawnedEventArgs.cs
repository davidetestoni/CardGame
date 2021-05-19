using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureSpawnedEventArgs : GameEventArgs
    {
        public CreatureCardInstance Creature { get; set; }
    }
}
