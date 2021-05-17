using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Models.PlayerActions.Cards.Creatures
{
    public class AttackPlayerAction : PlayerAction
    {
        public CardInstance Attacker { get; set; }
    }
}
