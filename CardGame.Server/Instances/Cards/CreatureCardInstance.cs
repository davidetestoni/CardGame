using CardGame.Server.Instances.Players;
using CardGame.Shared.Enums;
using System;

namespace CardGame.Server.Models.Cards.Instances
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
        public int Attack { get; set; } = 1;

        /// <summary>
        /// The current value of the health of this card.
        /// </summary>
        public int Health { get; set; } = 1;

        /// <summary>
        /// Gets the current card features.
        /// </summary>
        public CardFeature Features { get; set; } = CardFeature.None;
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
        /// <param name="damageSource">The card that damaged the target</param>
        /// <param name="damage">The amount of damage inflicted (before mitigation)</param>
        public virtual void OnCardDamaged(CreatureCardInstance target, CardInstance damageSource, int damage)
        {
            if (target == this)
            {
                if (Health > damage)
                {
                    Health -= damage;
                    Damaged?.Invoke(this, new DamagedEventArgs { DamageSource = damageSource, Value = damage });
                }
                else
                {
                    Health = 0;
                    Damaged?.Invoke(this, new DamagedEventArgs { DamageSource = damageSource, Value = damage });
                    Destroyed?.Invoke(this, new DestroyedEventArgs { DestructionSource = damageSource });
                }
            }
        }

        /// <summary>
        /// Called when a card is destroyed by another card.
        /// </summary>
        /// <param name="target">The card that was destroyed</param>
        /// <param name="destructionSource">The card that destroyed this card</param>
        public virtual void OnCardDestroyed(CreatureCardInstance target, CardInstance destructionSource)
        {
            if (target == this)
            {
                Destroyed?.Invoke(this, new DestroyedEventArgs { DestructionSource = destructionSource });
            }
        }

        /// <summary>
        /// Called after an attack has been performed.
        /// </summary>
        /// <param name="target">The card that has been attacked</param>
        /// <param name="damage">The damage dealt to the opponent</param>
        public virtual void OnAfterAttack(CreatureCardInstance target, int damage) { }

        /// <summary>
        /// Called at the beginning of a player's turn.
        /// </summary>
        /// <param name="player">The player that is currently playing the turn</param>
        /// <param name="turnNumber">The number of turns that passed since the start of the game</param>
        public virtual void OnTurnStart(PlayerInstance player, int turnNumber) { }

        /// <summary>
        /// Called at the end of a player's turn.
        /// </summary>
        /// <param name="player">The player that is currently playing the turn</param>
        /// <param name="turnNumber">The number of turns that passed since the start of the game</param>
        public virtual void OnTurnEnd(PlayerInstance player, int turnNumber) { }

        /// <summary>
        /// Called when a player draws cards from the deck.
        /// </summary>
        /// <param name="player">The player that drew the cards</param>
        /// <param name="count">The amount of cards that were drawn</param>
        /// <param name="isInitialDraw">Whether this is the initial draw that happens at the start of a player's turn</param>
        public virtual void OnCardsDrawn(PlayerInstance player, int count, bool isInitialDraw) { }

        /// <summary>
        /// Called when a card is placed on the field by a player.
        /// </summary>
        /// <param name="player">The player that placed the card</param>
        /// <param name="newCard">The card that just joined the field</param>
        public virtual void OnCreaturePlayed(PlayerInstance player, CreatureCardInstance newCard) { }
        #endregion
    }
}
