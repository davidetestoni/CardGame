using CardGame.Server.Models;

namespace CardGame.Server.Factories
{
    public class GameInstanceFactory
    {
        public GameInstance Create(GameInstanceOptions options, PlayerDefinition playerOne, PlayerDefinition playerTwo)
        {
            return new GameInstance()
            {
                Options = options,
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
