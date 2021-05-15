using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreaturePlayedEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public CreatureCardInstance Creature { get; set; }
    }
}
