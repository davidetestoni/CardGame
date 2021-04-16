using CardGame.Server.Instances.Game;
using CardGame.Shared.Models.Players;

namespace CardGame.Server.Factories
{
    public class GameInstanceFactory
    {
        private readonly CardInstanceFactory _cardFactory;
        private readonly PlayerInstanceFactory _playerFactory;

        public GameInstanceFactory(CardInstanceFactory cardFactory, PlayerInstanceFactory playerFactory)
        {
            _cardFactory = cardFactory;
            _playerFactory = playerFactory;
        }

        public GameInstance Create(GameInstanceOptions options, Player playerOne, Player playerTwo)
        {
            return new GameInstance(_cardFactory)
            {
                Options = options,
                PlayerOne = _playerFactory.Create(playerOne),
                PlayerTwo = _playerFactory.Create(playerTwo)
            };
        }
    }
}
