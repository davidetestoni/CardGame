using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Models.Cards;
using System.Linq;

namespace SampleGame.Tests.Extensions
{
    public static class PlayerInstanceExtensions
    {
        /// <summary>
        /// Helper method that gets a creature of a certain type on the player's field.
        /// </summary>
        public static CreatureCardInstance GetCreatureOnField<T>(this PlayerInstance player) where T : CreatureCard
            => player.Field.FirstOrDefault(c => c.Base is T);

        public static CreatureCard GetCreatureInHand<T>(this PlayerInstance player) where T : CreatureCard
            => player.Hand.FirstOrDefault(c => c is T) as CreatureCard;
    }
}
