using CardGame.Factories;
using CardGame.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace CardGame.Tests
{
    public class GameInstanceTests
    {
        [Fact]
        public void Test1()
        {

        }

        private GameInstance CreateSampleGame()
        {
            var factory = new GameInstanceFactory();
            var playerOne = new PlayerDefinition
            {
                Name = "Alice",
                Deck = new List<Card>
                {
                    // TODO: Add a sample deck
                }
            };
            var playerTwo = new PlayerDefinition
            {
                Name = "Bob",
                Deck = new List<Card>
                {
                    // TODO: Add a sample deck
                }
            };

            return factory.Create(playerOne, playerTwo);
        }
    }
}
