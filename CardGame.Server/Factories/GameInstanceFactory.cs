using CardGame.Server.Extensions;
using CardGame.Server.Instances.Cards;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Shared.Models.Players;
using System;
using System.Collections.Generic;

namespace CardGame.Server.Factories
{
    public class GameInstanceFactory
    {
        private readonly CardInstanceFactory cardInstanceFactory;

        public GameInstanceFactory(CardInstanceFactory cardInstanceFactory)
        {
            this.cardInstanceFactory = cardInstanceFactory;
        }

        public GameInstance Create(GameInstanceOptions options)
        {
            return new GameInstance()
            {
                Id = Guid.NewGuid(),
                Options = options
            };
        }

        public GameInstance Create(GameInstanceOptions options, Player playerOne, Player playerTwo)
        {
            var game = new GameInstance()
            {
                Id = Guid.NewGuid(),
                Options = options
            };

            game.PlayerOne = CreatePlayer(game, playerOne);
            game.PlayerTwo = CreatePlayer(game, playerTwo);

            return game;
        }

        public PlayerInstance CreatePlayer(GameInstance game, Player player)
        {
            var instance = new PlayerInstance
            {
                Id = player.Id,
                Name = player.Name,
                InitialHealth = game.Options.InitialHealth,
                CurrentHealth = game.Options.InitialHealth
            };

            var deck = new List<CardInstance>();

            foreach (var definition in player.Deck)
            {
                for (var i = 0; i < definition.Value; i++)
                {
                    deck.Add(cardInstanceFactory.Create(definition.Key, game, instance));
                }
            }

            // Try to use a somewhat random seed to prevent shuffling the decks of the
            // two players in the same exact way.
            var random = new Random(instance.Id.GetHashCode());
            deck.Shuffle(random);
            instance.Deck = deck;

            return instance;
        }
    }
}
