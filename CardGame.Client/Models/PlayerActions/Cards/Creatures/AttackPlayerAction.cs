using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Models.PlayerActions.Cards.Creatures
{
    /// <summary>
    /// Action used to attack the enemy player with one of your creatures.
    /// </summary>
    public class AttackPlayerAction : PlayerAction
    {
        /// <summary>
        /// The creature on your field that is performing the attack.
        /// </summary>
        public CreatureCardInstance Attacker { get; set; }
    }
}
