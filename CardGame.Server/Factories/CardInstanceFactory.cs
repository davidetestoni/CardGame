using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Enums;
using CardGame.Shared.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CardGame.Server.Factories
{
    /// <summary>
    /// This is the factory that provides instances of cards declared in an <see cref="Assembly"/>.
    /// </summary>
    public class CardInstanceFactory
    {
        private readonly Dictionary<Card, Type> _cards;

        /// <summary>
        /// Creates a factory basing on the cards declared in an <paramref name="assembly"/>.
        /// </summary>
        public CardInstanceFactory(Assembly assembly)
        {
            _cards = new Dictionary<Card, Type>();

            foreach (var type in assembly.GetTypes().Where(t => IsPlayableCard(t)))
            {
                var card = (Card)Activator.CreateInstance(type);
                var attr = type.GetCustomAttribute<PlayableCard>();
                _cards.Add(card, attr.InstanceType);
            }
        }

        /// <summary>
        /// Creates a <see cref="CardInstance"/> from a <paramref name="shortName"/>,
        /// and also sets its <paramref name="owner"/>.
        /// </summary>
        public CardInstance Create(string shortName, GameInstance game, PlayerInstance owner)
            => Create(_cards.Keys.First(c => c.ShortName == shortName), game, owner);

        /// <summary>
        /// Create a <see cref="CardInstance"/> from a base card of type <typeparamref name="T"/>,
        /// and also sets its <paramref name="owner"/>.
        /// </summary>
        public CardInstance Create<T>(GameInstance game, PlayerInstance owner) where T : Card
            => Create(_cards.Keys.First(c => c is T), game, owner);

        private CardInstance Create(Card card, GameInstance game, PlayerInstance owner)
        {
            CardInstance instance = null;

            switch (card)
            {
                case CreatureCard x:
                    instance = CreateCreature(x);
                    break;
            }

            instance.Game = game;
            instance.Owner = owner;
            instance.Id = Guid.NewGuid();
            instance.ManaCost = card.ManaCost;

            return instance;
        }

        private CreatureCardInstance CreateCreature(CreatureCard card)
        {
            var type = _cards[card];
            var instance = (CreatureCardInstance)Activator.CreateInstance(type);

            instance.Base = card;
            instance.Attack = card.Attack;
            instance.Health = card.Health;
            instance.Features = CardFeature.None;

            if (HasAttribute<Taunt>(card.GetType())) instance.Features |= CardFeature.Taunt;
            if (HasAttribute<Rush>(card.GetType())) instance.Features |= CardFeature.Rush;

            return instance;
        }

        private static bool IsPlayableCard(Type type)
            => type.CustomAttributes.Any(a => a.AttributeType == typeof(PlayableCard));

        private static bool HasAttribute<T>(Type type) where T : Attribute
            => type.GetCustomAttribute<T>() != null;
    }
}
