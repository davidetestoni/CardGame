using CardGame.Server.Instances.Cards;
using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class AdrianTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public AdrianTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void PlayCreatureFromHand_Adrian_EffectProcs()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetMana(2, 2);

            var adrian = _factoryFixture.CardFactory.Create<Adrian>(game, game.CurrentPlayer) as CreatureCardInstance;

            game
                .SetHands(adrian);

            // Play Adrian
            game.PlayCreatureFromHand(game.CurrentPlayer, adrian);

            Assert.Single(game.CurrentPlayer.Hand);
        }
    }
}
