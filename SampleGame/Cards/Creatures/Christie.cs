using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(ChristieInstance))]
    public class Christie : CreatureCard
    {
        public Christie()
        {
            ShortName = "Christie";
            Name = "Christie";
            Description = "Spawns 2 Rolands when destroyed";
            Art = "Christie";

            ManaCost = 5;
            Attack = 4;
            Health = 5;
        }
    }

    public class ChristieInstance : CreatureCardInstance
    {
        public override void OnCardDestroyed(CardInstance destructionSource, CreatureCardInstance target)
        {
            if (target == this)
            {
                Game.SpawnCreature("Roland", Owner);
                Game.SpawnCreature("Roland", Owner);
            }
        }
    }
}
