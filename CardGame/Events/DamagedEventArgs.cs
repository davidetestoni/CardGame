using CardGame.Models.Cards.Instances;
using System;

namespace CardGame.Events
{
    /// <summary>
    /// Provides information on the damage source and value.
    /// </summary>
    public class DamagedEventArgs : EventArgs
    {
        /// <summary>
        /// The card that caused the damage.
        /// </summary>
        public CardInstance DamageSource { get; set; }

        /// <summary>
        /// The value of the damage after external mitigation.
        /// </summary>
        public int Value { get; set; }
    }
}
