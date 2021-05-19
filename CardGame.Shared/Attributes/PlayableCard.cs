using System;

namespace CardGame.Shared.Attributes
{
    /// <summary>
    /// A playable card. Decorate classes that inherit from <see cref="Models.Cards.Card"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayableCard : Attribute
    {
        /// <summary>
        /// The type of instance that is created when the card
        /// is instanced in the game.
        /// </summary>
        public Type InstanceType { get; set; }

        /// <summary>
        /// Defines a playable card and the corresponding instance <paramref name="type"/> that
        /// is created when the card is instanced in the game.
        /// </summary>
        /// <param name="type"></param>
        public PlayableCard(Type type)
        {
            InstanceType = type;
        }
    }
}
