using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Models.PlayerActions.Cards.Creatures
{
    public class AttackCreatureAction : PlayerAction
    {
        public CreatureCardInstance Attacker { get; set; }
        public CreatureCardInstance Target { get; set; }
    }
}
