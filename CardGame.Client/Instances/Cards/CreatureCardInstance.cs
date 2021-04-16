using CardGame.Client.Events;
using System;

namespace CardGame.Client.Instances.Cards
{
    public class CreatureCardInstance : CardInstance
    {
        #region Events
        /// <summary>
        /// Called when this card is damaged. The argument is the amount of incoming damage,
        /// after mitigation due to external factors.
        /// </summary>
        public event EventHandler<DamagedEventArgs> Damaged;

        /// <summary>
        /// Called when this card is destroyed.
        /// </summary>
        public event EventHandler<DestroyedEventArgs> Destroyed;

        /// <summary>
        /// Called when this card attacks another card.
        /// </summary>
        public event EventHandler<AttackedEventArgs> Attacked;
        #endregion
    }
}
