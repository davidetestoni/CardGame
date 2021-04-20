using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(AttackerInstance))]
    public class Attacker : CreatureCard
    {
        public Attacker()
        {
            ShortName = "Attacker";
            Name = "Attacker";
            Description = "Description of a attacker. When attacking, deals 1 more damage.";

            ManaCost = 2;
            Attack = 1;
            Health = 2;
        }
    }

    public class AttackerInstance : CreatureCardInstance
    {
        public override int GetAttackDamage(CreatureCardInstance target) => Attack + 1;
    }
}
