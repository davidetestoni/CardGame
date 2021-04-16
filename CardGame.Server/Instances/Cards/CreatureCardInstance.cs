using CardGame.Server.Enums;
using CardGame.Server.Instances.Players;
using CardGame.Shared.Enums;

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
        /// The amount of times this card can attack during this turn.
        /// </summary>
        public int AttacksLeft { get; set; } = 0;

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
        /// Resets <see cref="AttacksLeft"/> for the current turn.
        /// </summary>
        public virtual void ResetAttacksLeft() { }

        /// <summary>
        /// Called when a card is damaged by another card (spell or creature).
        /// </summary>
        /// <param name="damageSource">The card that damaged the target</param>
        /// <param name="target">The card that was damaged</param>
        /// <param name="damage">The amount of damage inflicted (before mitigation)</param>
        public virtual void OnCardDamaged(CardInstance damageSource, CreatureCardInstance target, int damage)
        {
            if (target == this)
            {
                if (Health > damage)
                {
                    Health -= damage;
                    // TODO: Notify client about damage
                }
                else
                {
                    Health = 0;

                    // TODO: Notify client about damage
                    // TODO: Notify client about destruction
                }
            }
        }

        /// <summary>
        /// Called when a card is destroyed by another card.
        /// </summary>
        /// <param name="destructionSource">The card that destroyed this card</param>
        /// <param name="target">The card that was destroyed</param>
        public virtual void OnCardDestroyed(CardInstance destructionSource, CreatureCardInstance target)
        {
            // TODO: Notify client about destruction
        }

        /// <summary>
        /// Called before an attack is performed.
        /// </summary>
        /// <param name="attacker">The attacking card</param>
        /// <param name="target">The card that is being attacked</param>
        public virtual void OnBeforeAttack(CreatureCardInstance attacker, CreatureCardInstance target) { }

        /// <summary>
        /// Called after an attack has been performed.
        /// </summary>
        /// <param name="attacker">The attacking card</param>
        /// <param name="target">The card that has been attacked</param>
        /// <param name="damage">The damage dealt to the opponent</param>
        public virtual void OnAfterAttack(CreatureCardInstance attacker, CreatureCardInstance target, int damage) { }

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
        /// <param name="drawEventSource">The cause of the draw</param>
        public virtual void OnCardsDrawn(PlayerInstance player, int count, DrawEventSource drawEventSource = DrawEventSource.Effect) { }

        /// <summary>
        /// Called when a card is placed on the field by a player.
        /// </summary>
        /// <param name="player">The player that placed the card</param>
        /// <param name="newCard">The card that just joined the field</param>
        public virtual void OnCreaturePlayed(PlayerInstance player, CreatureCardInstance newCard) { }
        #endregion
    }
}
