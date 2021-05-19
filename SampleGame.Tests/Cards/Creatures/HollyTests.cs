using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class HollyTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public HollyTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void OnAfterAttack_Holly_EffectProcs()
        {
            var game = 
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Simon(), new Holly())
                .ResetAttacksLeft();

            // Attack Holly with Simon
            var simon = game.CurrentPlayer.GetCreatureOnField<Simon>();
            var holly = game.Opponent.GetCreatureOnField<Holly>();
            game.AttackCreature(game.CurrentPlayer, simon, holly);

            Assert.Equal(holly.Base.Health, holly.Health);
            Assert.Equal(simon.Base.Health - 1, simon.Health);
        }
    }
}
