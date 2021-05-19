using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreatureSpawnedEventArgs : GameEventArgs
    {
        public CreatureCardInstance Creature { get; set; }
    }
}
