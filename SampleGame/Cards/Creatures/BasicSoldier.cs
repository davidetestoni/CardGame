using CardGame.Server.Instances.Cards;
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
            Art = "https://image.winudf.com/v2/image1/Y29tLndhbGxpZS5taWxpdGFyeS53YWxscGFwZXJzX3NjcmVlbl8yXzE1ODU4NTMxNTFfMDkx/screen-2.jpg?fakeurl=1&type=.jpg";
            
            ManaCost = 1;
            Attack = 1;
            Health = 1;
        }
    }
}
