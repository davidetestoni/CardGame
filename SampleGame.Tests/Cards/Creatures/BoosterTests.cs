using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class BoosterTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public BoosterTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void PlayCreatureFromHand_Booster_EffectProcs()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetMana(2, 2)
                .SetHands(new Booster());

            // Play the booster
            var booster = game.CurrentPlayer.GetCreatureInHand<Booster>();
            game.PlayCreatureFromHand(game.CurrentPlayer, booster);

            Assert.Single(game.CurrentPlayer.Hand);
        }
    }
}
