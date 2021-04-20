using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(BoosterInstance))]
    public class Booster : CreatureCard
    {
        public Booster()
        {
            ShortName = "Booster";
            Name = "Booster";
            Description = "Description of a booster. When you play this creature, draw a card";

            ManaCost = 2;
            Attack = 1;
            Health = 1;
        }
    }

    public class BoosterInstance : CreatureCardInstance
    {
        public override void OnCreaturePlayed(PlayerInstance player, CreatureCardInstance newCard)
        {
            base.OnCreaturePlayed(player, newCard);

            if (newCard == this)
            {
                Game.DrawCards(player, 1);
            }
        }
    }
}
