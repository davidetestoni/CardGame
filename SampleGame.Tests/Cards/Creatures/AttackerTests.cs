using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class AttackerTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public AttackerTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void AttackCreature_Attacker_EffectProcs()
        {
            var game = 
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new Attacker(), new Gunner())
                .ResetAttacksLeft();

            // Attack the gunner with the attacker
            var attacker = game.CurrentPlayer.GetCreatureOnField<Attacker>();
            var gunner = game.Opponent.GetCreatureOnField<Gunner>();
            game.AttackCreature(game.CurrentPlayer, attacker, gunner);

            Assert.Equal(1, gunner.Health);
            Assert.Equal(1, attacker.Health);
        }
    }
}
