using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;
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
        private readonly List<Card> _cards;

        /// <summary>
        /// Creates a factory basing on the cards declared in an <paramref name="assembly"/>.
        /// </summary>
        public CardInstanceFactory(Assembly assembly)
        {
            _cards = new List<Card>();

            foreach (var type in assembly.GetTypes().Where(t => IsPlayableCard(t)))
            {
                var card = (Card)Activator.CreateInstance(type);
                _cards.Add(card);
            }
        }

        /// <summary>
        /// Create a <see cref="CardInstance"/> from a <paramref name="shortName"/> and an <paramref name="id"/>.
        /// </summary>
        public CardInstance Create(string shortName, Guid id, PlayerInstance owner)
            => Create(_cards.First(c => c.ShortName == shortName), id, owner);

        /// <summary>
        /// Create a <see cref="CardInstance"/> from a base card of type <typeparamref name="T"/> and an <paramref name="id"/>.
        /// </summary>
        public CardInstance Create<T>(Guid id, PlayerInstance owner) where T : Card
            => Create(_cards.First(c => c is T), id, owner);

        private CardInstance Create(Card card, Guid id, PlayerInstance owner)
        {
            CardInstance instance = null;

            switch (card)
            {
                case CreatureCard x:
                    instance = CreateCreature(x);
                    break;
            }

            instance.Id = id;
            instance.Owner = owner;
            instance.ManaCost = card.ManaCost;

            return instance;
        }

        private CreatureCardInstance CreateCreature(CreatureCard card)
        {
            var instance = new CreatureCardInstance
            {
                Base = card,
                Attack = card.Attack,
                Health = card.Health,
                Features = CardFeature.None
            };

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
