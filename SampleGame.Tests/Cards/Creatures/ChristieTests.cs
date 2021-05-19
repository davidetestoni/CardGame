using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class ChristieTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public ChristieTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void PlayCreatureFromHand_Christie_EffectProcs()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Christie());

            // Destroy Christie
            var christie = game.CurrentPlayer.GetCreatureOnField<Christie>();
            game.DestroyCreature(null, christie);

            Assert.Equal(2, game.CurrentPlayer.Field.Count);
            Assert.Equal("Roland", game.CurrentPlayer.Field[0].ShortName);
            Assert.Equal("Roland", game.CurrentPlayer.Field[1].ShortName);
        }
    }
}
