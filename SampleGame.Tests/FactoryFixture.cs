using CardGame.Server.Factories;
using CardGame.Server.Instances.Game;
using CardGame.Shared.Models.Cards;
using CardGame.Shared.Models.Players;
using SampleGame.Cards.Creatures;
using System.Collections.Generic;

namespace SampleGame.Tests
{
    public class FactoryFixture
    {
        public CardInstanceFactory CardFactory { get; private set; }
        public PlayerInstanceFactory PlayerFactory { get; private set; }
        public GameInstanceFactory GameFactory { get; private set; }

        public FactoryFixture()
        {
            CardFactory = new(typeof(BasicSoldier).Assembly);
            PlayerFactory = new();
            GameFactory = new(CardFactory, PlayerFactory);
        }

        /// <summary>
        /// Creates a test game with 2 players with test decks made of <paramref name="deckSize"/>
        /// copies of <see cref="BasicSoldier"/>.
        /// </summary>
        public GameInstance CreateTestGame(int deckSize = 20)
        {
            var options = new GameInstanceOptions();
            var playerOne = CreateTestPlayer("Player 1", deckSize);
            var playerTwo = CreateTestPlayer("Player 2", deckSize);

            return GameFactory.Create(options, playerOne, playerTwo);
        }

        // Creates a test player with a deck made only of basic soldiers
        // Use this to test basic mechanics of the game
        private static Player CreateTestPlayer(string name, int deckSize)
            => new()
            {
                Name = name,
                Deck = new List<(Card, int)>
                {
                    (new BasicSoldier(), deckSize)
                }
            };

        /// <summary>
        /// Creates a sample game with 2 players and test decks.
        /// </summary>
        public GameInstance CreateSampleGame()
        {
            var options = new GameInstanceOptions();
            var playerOne = CreateSamplePlayer("Player 1");
            var playerTwo = CreateSamplePlayer("Player 2");

            return GameFactory.Create(options, playerOne, playerTwo);
        }

        // Creates a sample player with a working deck
        private static Player CreateSamplePlayer(string name)
            => new()
            {
                Name = name,
                Deck = new List<(Card, int)>
                {
                    (new BasicSoldier(), 4),
                    (new Gunner(), 3),
                    (new Defender(), 3)
                }
            };
    }
}
