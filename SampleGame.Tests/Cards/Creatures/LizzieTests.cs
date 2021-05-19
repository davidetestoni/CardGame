using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class LizzieTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public LizzieTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void AttackCreature_Attacker_EffectProcs()
        {
            var game = 
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Lizzie(), new Simon())
                .ResetAttacksLeft();

            // Attack Simon with Lizzie
            var lizzie = game.CurrentPlayer.GetCreatureOnField<Lizzie>();
            var simon = game.Opponent.GetCreatureOnField<Simon>();
            game.AttackCreature(game.CurrentPlayer, lizzie, simon);

            Assert.Equal(1, simon.Health);
            Assert.Equal(1, lizzie.Health);
        }
    }
}
