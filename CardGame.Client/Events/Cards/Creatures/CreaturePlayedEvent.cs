using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Cards.Creatures
{
    public class CreaturePlayedEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public CreatureCardInstance Creature { get; set; }
    }
}
