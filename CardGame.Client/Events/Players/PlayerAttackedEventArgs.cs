using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Players
{
    public class PlayerAttackedEventArgs : GameEventArgs
    {
        public PlayerInstance Player { get; set; }
        public CreatureCardInstance Attacker { get; set; }
        public int Damage { get; set; }
    }
}
