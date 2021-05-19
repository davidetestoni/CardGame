using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(LizzieInstance))]
    public class Lizzie : CreatureCard
    {
        public Lizzie()
        {
            ShortName = "Lizzie";
            Name = "Lizzie";
            Description = "When attacking, deals 1 more damage.";
            Art = "Lizzie";

            ManaCost = 2;
            Attack = 1;
            Health = 2;
        }
    }

    public class LizzieInstance : CreatureCardInstance
    {
        public override int GetAttackDamage(CreatureCardInstance target, bool isAttacking)
            => isAttacking ? Attack + 1 : Attack;
    }
}
