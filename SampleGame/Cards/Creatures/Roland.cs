using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance))]
    public class Roland : CreatureCard
    {
        public Roland()
        {
            ShortName = "Roland";
            Name = "Roland";
            Description = "Basic unit, doesn't do much";
            Art = "Roland";
            
            ManaCost = 1;
            Attack = 1;
            Health = 1;
        }
    }
}
