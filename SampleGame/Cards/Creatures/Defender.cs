using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance)), Taunt]
    public class Defender : CreatureCard
    {
        public Defender()
        {
            ShortName = "Defender";
            Name = "Defender";
            Description = "Description of a defender";
            Art = "https://i.pinimg.com/736x/33/b7/f0/33b7f027d77b122b9eee772b190b5860.jpg";

            ManaCost = 3;
            Attack = 2;
            Health = 2;
        }
    }
}
