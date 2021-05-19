using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance)), Rush]
    public class Evie : CreatureCard
    {
        public Evie()
        {
            ShortName = "Evie";
            Name = "Evie";
            Description = "Rush";
            Art = "Evie";

            ManaCost = 2;
            Attack = 1;
            Health = 1;
        }
    }
}
