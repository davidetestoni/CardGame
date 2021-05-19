using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance))]
    public class Simon : CreatureCard
    {
        public Simon()
        {
            ShortName = "Simon";
            Name = "Simon";
            Description = "Fortified unit";
            Art = "Simon";

            ManaCost = 2;
            Attack = 1;
            Health = 3;
        }
    }
}
