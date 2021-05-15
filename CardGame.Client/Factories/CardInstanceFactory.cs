using CardGame.Client.Instances.Cards;
using CardGame.Shared.Attributes;
using CardGame.Shared.Enums;
using CardGame.Shared.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CardGame.Client.Factories
{
    /// <summary>
    /// This is the factory that provides instances of cards basing on their base values.
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
        /// Create a <see cref="CardInstance"/> from a <paramref name="shortName"/> and an <paramref name="id"/>.
        /// </summary>
        public CardInstance Create(string shortName, Guid id)
            => Create(_cards.Keys.First(c => c.ShortName == shortName), id);

        /// <summary>
        /// Create a <see cref="CardInstance"/> from a base card of type <typeparamref name="T"/> and an <paramref name="id"/>.
        /// </summary>
        public CardInstance Create<T>(Guid id) where T : Card
            => Create(_cards.Keys.First(c => c is T), id);

        private CardInstance Create(Card card, Guid id)
        {
            CardInstance instance = null;

            switch (card)
            {
                case CreatureCard x:
                    instance = CreateCreature(x);
                    break;
            }

            instance.Id = id;
            instance.ManaCost = card.ManaCost;

            return instance;
        }

        private CreatureCardInstance CreateCreature(CreatureCard card)
        {
            var type = _cards[card];
            var instance = (CreatureCardInstance)Activator.CreateInstance(type);

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
