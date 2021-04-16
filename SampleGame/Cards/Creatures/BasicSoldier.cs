using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance))]
    public class BasicSoldier : CreatureCard
    {
        public BasicSoldier()
        {
            ShortName = "BasicSoldier";
            Name = "Basic Soldier";
            Description = "Description of a basic soldier";
            
            ManaCost = 1;
            Attack = 1;
            Health = 1;
        }
    }
}
