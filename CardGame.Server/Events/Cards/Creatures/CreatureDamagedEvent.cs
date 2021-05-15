using CardGame.Server.Models.Cards.Instances;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureDamagedEvent : GameEvent
    {
        public CardInstance Target { get; set; }
        public int Damage { get; set; }
    }
}
