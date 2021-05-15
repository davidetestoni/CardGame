using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance))]
    public class Gunner : CreatureCard
    {
        public Gunner()
        {
            ShortName = "Gunner";
            Name = "Gunner";
            Description = "Description of a gunner";

            ManaCost = 2;
            Attack = 1;
            Health = 3;
        }
    }
}
