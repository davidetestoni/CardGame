using CardGame.Server.Extensions;
using CardGame.Server.Instances.Cards;
using CardGame.Server.Instances.Players;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(LennyInstance)), Taunt]
    public class Lenny : CreatureCard
    {
        public Lenny()
        {
            ShortName = "Lenny";
            Name = "Lenny";
            Description = "Taunt. When played, increase the attack of a random ally by 1";
            Art = "Lenny";

            ManaCost = 3;
            Attack = 2;
            Health = 2;
        }
    }

    public class LennyInstance : CreatureCardInstance
    {
        public override void OnCreaturePlayed(PlayerInstance player, CreatureCardInstance newCard)
        {
            base.OnCreaturePlayed(player, newCard);

            if (player.Field.Count > 0)
            {
                Game.ChangeCreatureAttack(player.Field.GetRandom(Game.Random), 1);
            }
        }
    }
}
