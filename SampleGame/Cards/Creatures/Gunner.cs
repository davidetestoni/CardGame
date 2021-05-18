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
            Art = "https://i.pinimg.com/736x/00/49/49/0049493e924ba1195e2f9dccfb049481.jpg";

            ManaCost = 2;
            Attack = 1;
            Health = 3;
        }
    }
}
