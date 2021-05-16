using CardGame.Server.Extensions;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Models.Cards;
using CardGame.Shared.Models.Players;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using CardGame.Shared.Attributes;

namespace CardGame.Server.Factories
{
    /// <summary>
    /// This is the factory that provides instances of players of a <see cref="GameInstance"/>.
    /// </summary>
    public class PlayerInstanceFactory
    {
        private readonly List<Card> _cards;

        /// <summary>
        /// Creates a factory basing on the cards declared in an <paramref name="assembly"/>.
        /// </summary>
        public PlayerInstanceFactory(Assembly assembly)
        {
            _cards = new List<Card>();

            foreach (var type in assembly.GetTypes().Where(t => IsPlayableCard(t)))
            {
                var card = (Card)Activator.CreateInstance(type);
                _cards.Add(card);
            }
        }

        /// <summary>
        /// Creates a <see cref="PlayerInstance"/> basing on a <paramref name="player"/>.
        /// </summary>
        public PlayerInstance Create(Player player)
        {
            var instance = new PlayerInstance
            {
                Id = player.Id,
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
                for (var i = 0; i < definition.Value; i++)
                {
                    deck.Add(_cards.First(c => c.ShortName == definition.Key));
                }
            }

            // Try to use a somewhat random seed to prevent shuffling the decks of the
            // two players in the same exact way.
            var random = new Random(instance.Id.GetHashCode());
            deck.Shuffle(random);
            instance.Deck = deck;

            return instance;
        }

        private static bool IsPlayableCard(Type type)
            => type.CustomAttributes.Any(a => a.AttributeType == typeof(PlayableCard));
    }
}
