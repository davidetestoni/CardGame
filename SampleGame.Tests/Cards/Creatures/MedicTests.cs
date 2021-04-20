using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class MedicTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public MedicTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void OnAfterAttack_Medic_EffectProcs()
        {
            var game = 
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new Gunner(), new Medic())
                .ResetAttacksLeft();

            // Attack the medic with the gunner
            var gunner = game.CurrentPlayer.GetCreatureOnField<Gunner>();
            var medic = game.Opponent.GetCreatureOnField<Medic>();
            game.AttackCreature(game.CurrentPlayer, gunner, medic);

            Assert.Equal(medic.Base.Health, medic.Health);
            Assert.Equal(gunner.Base.Health - 1, gunner.Health);
        }
    }
}
