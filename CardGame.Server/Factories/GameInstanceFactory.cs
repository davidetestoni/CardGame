using CardGame.Server.Extensions;
using CardGame.Server.Instances.Cards;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Shared.Models.Players;
using System;
using System.Collections.Generic;

namespace CardGame.Server.Factories
{
    /// <summary>
    /// Takes care of creating new games.
    /// </summary>
    public class GameInstanceFactory
    {
        private readonly CardInstanceFactory cardInstanceFactory;

        public GameInstanceFactory(CardInstanceFactory cardInstanceFactory)
        {
            this.cardInstanceFactory = cardInstanceFactory;
        }

        /// <summary>
        /// Creates a game with the given <paramref name="options"/>, without setting the players.
        /// </summary>
        public GameInstance Create(GameInstanceOptions options)
        {
            return new GameInstance(cardInstanceFactory)
            {
                Id = Guid.NewGuid(),
                Options = options
            };
        }

        /// <summary>
        /// Creates a game with the given <paramref name="options"/> and two players (<paramref name="playerOne"/>
        /// and <paramref name="playerTwo"/>).
        /// </summary>
        public GameInstance Create(GameInstanceOptions options, Player playerOne, Player playerTwo)
        {
            var game = new GameInstance(cardInstanceFactory)
            {
                Id = Guid.NewGuid(),
                Options = options
            };

            game.PlayerOne = AddPlayer(game, playerOne);
            game.PlayerTwo = AddPlayer(game, playerTwo);

            return game;
        }

        /// <summary>
        /// Creates a <see cref="PlayerInstance"/> from a <paramref name="player"/>
        /// and adds it to a <paramref name="game"/>.
        /// </summary>
        public PlayerInstance AddPlayer(GameInstance game, Player player)
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
