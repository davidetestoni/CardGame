using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Models.PlayerActions.Cards.Creatures
{
    public class AttackCreatureAction : PlayerAction
    {
        public CardInstance Attacker { get; set; }
        public CardInstance Target { get; set; }
    }
}
