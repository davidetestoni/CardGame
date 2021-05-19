using CardGame.Server.Extensions;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(HollyInstance))]
    public class Holly : CreatureCard
    {
        public Holly()
        {
            ShortName = "Holly";
            Name = "Holly";
            Description = "After being attacked, restores 2 HP to a random ally.";
            Art = "";

            ManaCost = 4;
            Attack = 1;
            Health = 6;
        }
    }

    public class HollyInstance : CreatureCardInstance
    {
        public override void OnAfterAttack(CreatureCardInstance attacker, CreatureCardInstance target, int damage)
        {
            base.OnAfterAttack(attacker, target, damage);

            if (target == this && Health > 0)
            {
                var cardToHeal = Owner.Field.GetRandom(Game.Random);
                Game.RestoreCreatureHealth(cardToHeal, 2);
            }
        }
    }
}
