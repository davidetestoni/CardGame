using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
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
                PlayerOne = CreatePlayer(options, playerOne),
                PlayerTwo = CreatePlayer(options, playerTwo)
            };
        }

        private PlayerInstance CreatePlayer(GameInstanceOptions options, Player player)
        {
            var instance = _playerFactory.Create(player);
            instance.InitialHealth = options.InitialHealth;
            instance.CurrentHealth = options.InitialHealth;

            return instance;
        }
    }
}
