using CardGame.Server.Extensions;
using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(MedicInstance))]
    public class Medic : CreatureCard
    {
        public Medic()
        {
            ShortName = "Medic";
            Name = "Medic";
            Description = "Description of a medic. After being attacked, restores 2 HP to a random creature on your field.";

            ManaCost = 3;
            Attack = 1;
            Health = 3;
        }
    }

    public class MedicInstance : CreatureCardInstance
    {
        public override void OnAfterAttack(CreatureCardInstance attacker, CreatureCardInstance target, int damage)
        {
            if (target == this && Health > 0)
            {
                var cardToHeal = Owner.Field.GetRandom(Game.Random);
                Game.RestoreCreatureHealth(cardToHeal, 2);
            }
        }
    }
}
