using CardGame.Server.Models.Cards.Instances;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureDestroyedEvent : GameEvent
    {
        public CardInstance Destroyer { get; set; }
        public CreatureCardInstance Target { get; set; }
    }
}
