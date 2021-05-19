using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Models.PlayerActions.Cards.Creatures
{
    /// <summary>
    /// Action used to attack an enemy creature with one of your creatures.
    /// </summary>
    public class AttackCreatureAction : PlayerAction
    {
        /// <summary>
        /// The creature on your field that is performing the attack.
        /// </summary>
        public CreatureCardInstance Attacker { get; set; }

        /// <summary>
        /// The target creature on your opponent's field that is receiving the attack.
        /// </summary>
        public CreatureCardInstance Target { get; set; }
    }
}
