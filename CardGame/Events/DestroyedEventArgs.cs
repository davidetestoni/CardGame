using CardGame.Models.Cards.Instances;
using System;

namespace CardGame.Events
{
    /// <summary>
    /// Provides information on the destruction source.
    /// </summary>
    public class DestroyedEventArgs : EventArgs
    {
        /// <summary>
        /// The card that caused the destruction.
        /// </summary>
        public CardInstance DamageSource { get; set; }
    }
}
