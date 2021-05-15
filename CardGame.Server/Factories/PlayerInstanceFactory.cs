using CardGame.Server.Extensions;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Models.Cards;
using CardGame.Shared.Models.Players;
using System;
using System.Collections.Generic;

namespace CardGame.Server.Factories
{
    /// <summary>
    /// This is the factory that provides instances of players of a <see cref="GameInstance"/>.
    /// </summary>
    public class PlayerInstanceFactory
    {
        /// <summary>
        /// Creates a <see cref="PlayerInstance"/> basing on a <paramref name="player"/>.
        /// </summary>
        public PlayerInstance Create(Player player)
        {
            var instance = new PlayerInstance
            {
                Id = Guid.NewGuid(),
                Name = player.Name,
                CurrentMana = 0,
                MaximumMana = 0,
                Field = new List<CreatureCardInstance>(),
                Graveyard = new List<Card>(),
                Hand = new List<CardInstance>()
            };

            var deck = new List<Card>();

            foreach (var definition in player.Deck)
            {
                for (var i = 0; i < definition.Item2; i++)
                {
                    deck.Add(definition.Item1);
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
