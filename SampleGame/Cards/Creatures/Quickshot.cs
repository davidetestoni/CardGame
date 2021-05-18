using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance)), Rush]
    public class Quickshot : CreatureCard
    {
        public Quickshot()
        {
            ShortName = "Quickshot";
            Name = "Quickshot";
            Description = "Description of a quickshot";
            Art = "https://i.pinimg.com/736x/d4/c2/4b/d4c24bddd80bbefa037407b8f65c4ccc.jpg";

            ManaCost = 2;
            Attack = 1;
            Health = 1;
        }
    }
}
