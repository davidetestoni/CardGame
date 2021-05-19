using CardGame.Server.Instances.Cards;
using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using Xunit;

namespace SampleGame.Tests.Cards.Creatures
{
    public class LennyTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public LennyTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        [Fact]
        public void PlayCreatureFromHand_Lenny_EffectProcs()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetMana(3, 3);

            var lenny = _factoryFixture.CardFactory.Create<Lenny>(game, game.CurrentPlayer) as CreatureCardInstance;

            game
                .SetHands(lenny);

            // Play Lenny
            game.PlayCreatureFromHand(game.CurrentPlayer, lenny);

            Assert.Equal(lenny.Base.Attack + 1, lenny.Attack);
        }
    }
}
