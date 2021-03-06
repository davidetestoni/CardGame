using CardGame.Models;

namespace CardGame.Factories
{
    public class GameInstanceFactory
    {
        /// <summary>
        /// The maximum amount of mana players can gain.
        /// </summary>
        public int MaximumMana { get; set; } = 10;

        /// <summary>
        /// The maximum amount of cards players can have in their hand.
        /// </summary>
        public int MaximumHandSize { get; set; } = 10;

        /// <summary>
        /// The initial amount of cards that will be drawn.
        /// </summary>
        public int InitialHandSize { get; set; } = 5;

        public GameInstance Create(PlayerDefinition playerOne, PlayerDefinition playerTwo)
        {
            return new GameInstance()
            {
                PlayerOne = new()
                {
                    Name = playerOne.Name,
                    Deck = playerOne.Deck
                },
                PlayerTwo = new()
                {
                    Name = playerTwo.Name,
                    Deck = playerTwo.Deck
                }
            };
        }
    }
}
