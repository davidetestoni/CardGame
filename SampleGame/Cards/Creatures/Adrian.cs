using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(AdrianInstance))]
    public class Adrian : CreatureCard
    {
        public Adrian()
        {
            ShortName = "Adrian";
            Name = "Adrian";
            Description = "When you play Adrian, draw a card";
            Art = "Adrian";

            ManaCost = 2;
            Attack = 1;
            Health = 2;
        }
    }

    public class AdrianInstance : CreatureCardInstance
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
