using CardGame.Server.Enums;
using CardGame.Server.Instances.Players;
using CardGame.Shared.Enums;
using CardGame.Shared.Models.Cards;

namespace CardGame.Server.Instances.Cards
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
        /// This is the original card from which the instance was created.
        /// </summary>
        public CreatureCard Base { get; set; }

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

        /// <inheritdoc/>
        public override string ShortName => Base.ShortName;
        #endregion

        // -----------------
        //  VIRTUAL METHODS
        // -----------------
        // When something happens on the field, these are called. Advanced cards can override
        // these to provide additional features.

        #region Virtual Methods
        /// <summary>
        /// Called before this card attacks another card. Returns the damage dealt by this card.
        /// </summary>
        /// <param name="target">The card that is being attacked</param>
        public virtual int GetAttackDamage(CreatureCardInstance target, bool isDefending) => Attack;

        /// <summary>
        /// Called when this card is attacked by another card. Return the damage taken.
        /// </summary>
        public virtual int ComputeDamageTaken(CreatureCardInstance attacker, int attackDamage, bool isDefending) => attackDamage;

        /// <summary>
        /// Called before this card attacks a player. Returns the damage dealt by this card.
        /// </summary>
        /// <param name="target">The player that is being attacked</param>
        public virtual int GetAttackDamage(PlayerInstance target) => Attack;

        /// <summary>
        /// Resets <see cref="AttacksLeft"/> for the current turn.
        /// </summary>
        public virtual void ResetAttacksLeft()
        {
            AttacksLeft = 1;
        }

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
                }
                else
                {
                    Health = 0;
                }
            }

            // TODO: Notify client about damage
        }

        /// <summary>
        /// Called when a player is damaged by a card.
        /// </summary>
        /// <param name="damageSource">The card that damaged the target</param>
        /// <param name="target">The player that was damaged</param>
        /// <param name="damage">The amount of damage inflicted (before mitigation)</param>
        public virtual void OnPlayerDamaged(CardInstance damageSource, PlayerInstance target, int damage) { }        

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
        /// Called before an attack is performed against a card.
        /// </summary>
        /// <param name="attacker">The attacking card</param>
        /// <param name="target">The card that is being attacked</param>
        public virtual void OnBeforeAttack(CreatureCardInstance attacker, CreatureCardInstance target) { }

        /// <summary>
        /// Called before an attack is performed against a player.
        /// </summary>
        /// <param name="attacker">The attacking card</param>
        /// <param name="target">The player that is being attacked</param>
        public virtual void OnBeforeAttack(CreatureCardInstance attacker, PlayerInstance target) { }

        /// <summary>
        /// Called after an attack has been performed against a card.
        /// </summary>
        /// <param name="attacker">The attacking card</param>
        /// <param name="target">The card that has been attacked</param>
        /// <param name="damage">The damage dealt to the opponent</param>
        public virtual void OnAfterAttack(CreatureCardInstance attacker, CreatureCardInstance target, int damage) { }

        /// <summary>
        /// Called after an attack is performed against a player.
        /// </summary>
        /// <param name="attacker">The attacking card</param>
        /// <param name="target">The player that has been attacked</param>
        /// <param name="damage">The damage dealt to the opponent</param>
        public virtual void OnAfterAttack(CreatureCardInstance attacker, PlayerInstance target, int damage) { }

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

        /// <summary>
        /// Called when a card is healed by an effect.
        /// </summary>
        /// <param name="target">The card that was healed</param>
        /// <param name="amount">The amount of health received</param>
        public virtual void OnCreatureHealed(CreatureCardInstance target, int amount) { }

        /// <summary>
        /// Called when a player is healed by an effect.
        /// </summary>
        /// <param name="target">The player that was healed</param>
        /// <param name="amount">The amount of health received</param>
        public virtual void OnPlayerHealed(PlayerInstance target, int amount) { }
        #endregion
    }
}
