using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Shared.Models.Players;
using System;

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

        public GameInstance Create(GameInstanceOptions options)
        {
            return new GameInstance(_cardFactory)
            {
                Id = Guid.NewGuid(),
                Options = options
            };
        }

        public GameInstance Create(GameInstanceOptions options, Player playerOne, Player playerTwo)
        {
            return new GameInstance(_cardFactory)
            {
                Id = Guid.NewGuid(),
                Options = options,
                PlayerOne = CreatePlayer(options, playerOne),
                PlayerTwo = CreatePlayer(options, playerTwo)
            };
        }

        public PlayerInstance CreatePlayer(GameInstanceOptions options, Player player)
        {
            var instance = _playerFactory.Create(player);
            instance.InitialHealth = options.InitialHealth;
            instance.CurrentHealth = options.InitialHealth;

            return instance;
        }
    }
}
