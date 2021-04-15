using CardGame.Events;
using System;

namespace CardGame.Models.Cards.Instances
{
    public class CreatureCardInstance : CardInstance
    {
        // ------------
        //  PROPERTIES
        // ------------
        // These represent the current stats of a card, and they can be changed during
        // the course of the game.

        #region Properties
        /// <summary>
        /// The current value of the attack of this card.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// The current value of the health of this card.
        /// </summary>
        public int Health { get; set; }
        #endregion

        // --------
        //  EVENTS
        // --------
        // Events are useful to display information about the attack.
        // Do not use events to proc effects, because they are not waited!

        #region Events
        /// <summary>
        /// Called when this card is damaged. The argument is the amount of incoming damage,
        /// after mitigation due to external factors.
        /// </summary>
        public event EventHandler<DamagedEventArgs> Damaged;

        /// <summary>
        /// Called when this card is destroyed
        /// </summary>
        public event EventHandler Destroyed;
        #endregion

        // -----------------
        //  VIRTUAL METHODS
        // -----------------
        // When something happens on the field, these are called. Advanced cards can override
        // these to provide additional features.

        #region Virtual Methods
        /// <summary>
        /// Called before this card attacks a target. Returns the damage dealt by this card.
        /// </summary>
        /// <param name="target">The card that is being attacked</param>
        public virtual int GetAttackDamage(CreatureCardInstance target) => Attack;

        /// <summary>
        /// Called when a card is damaged by another card (spell or creature).
        /// </summary>
        /// <param name="target">The card that was damaged</param>
        /// <param name="damagingCard">The card that damaged the target</param>
        /// <param name="damage">The amount of damage inflicted (before mitigation)</param>
        public virtual void OnCardDamaged(Card target, Card damagingCard, int damage)
        {
            if (target == this)
            {
                if (Health > damage)
                {
                    Health -= damage;
                    Damaged?.Invoke(this, damage);
                }
                else
                {
                    Health = 0;
                    Damaged?.Invoke(this, damage);
                    Destroyed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Called when a card is destroyed by another card.
        /// </summary>
        /// <param name="target">The card that was destroyed</param>
        /// <param name="destroyer">The card that destroyed this card</param>
        public virtual void OnCardDestroyed(Card target, Card destroyer)
        {
            if (target == this)
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called after an attack has been performed.
        /// </summary>
        /// <param name="target">The card that has been attacked</param>
        /// <param name="damage">The damage dealt to the opponent</param>
        public virtual void OnAfterAttack(Card target, int damage) { }

        /// <summary>
        /// Called at the beginning of a player's turn.
        /// </summary>
        /// <param name="player">The player that is currently playing the turn</param>
        /// <param name="turnNumber">The number of turns that passed since the start of the game</param>
        public virtual void OnTurnStart(Player player, int turnNumber) { }

        /// <summary>
        /// Called at the end of a player's turn.
        /// </summary>
        /// <param name="player">The player that is currently playing the turn</param>
        /// <param name="turnNumber">The number of turns that passed since the start of the game</param>
        public virtual void OnTurnEnd(Player player, int turnNumber) { }

        /// <summary>
        /// Called when a player draws cards from the deck.
        /// </summary>
        /// <param name="player">The player that drew the cards</param>
        /// <param name="count">The amount of cards that were drawn</param>
        /// <param name="isInitialDraw">Whether this is the initial draw that happens at the start of a player's turn</param>
        public virtual void OnCardsDrawn(Player player, int count, bool isInitialDraw) { }

        /// <summary>
        /// Called when a card is placed on the field by a player.
        /// </summary>
        /// <param name="player">The player that placed the card</param>
        /// <param name="newCard">The card that just joined the field</param>
        public virtual void OnCardPlayed(Player player, Card newCard) { }
        #endregion
    }
}
