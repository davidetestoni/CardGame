using CardGame.Server.Models.Cards.Instances;
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

            ManaCost = 2;
            Attack = 1;
            Health = 1;
        }
    }
}
