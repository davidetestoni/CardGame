using System;
using System.Collections.Generic;

namespace CardGame.Models
{
    public class CreatureCard : Card
    {
        #region Public Properties
        /// <summary>
        /// The base value of the attack of this card.
        /// </summary>
        public int BaseAttack { get; set; }

        /// <summary>
        /// The current value of the attack of this card.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// The base value of the health of this card.
        /// </summary>
        public int BaseHealth { get; set; }

        /// <summary>
        /// The current value of the health of this card.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// The effects of this card.
        /// </summary>
        public List<Effect> Effects { get; set; }
        #endregion

        #region Events
        public event EventHandler<int> Damaged;
        public event EventHandler Destroyed;
        #endregion

        #region Constructors
        public CreatureCard()
        {

        }

        public CreatureCard(string id, string name, int manaCost = 1, int baseAttack = 1, int baseHealth = 1, string description = "")
        {
            // Metadata
            Id = id;
            Name = name;
            Description = description;

            // Mana
            ManaCost = manaCost;

            // Attack
            BaseAttack = baseAttack;
            Attack = baseAttack;

            // Health
            BaseHealth = baseHealth;
            Health = baseHealth;
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Called when this card is damaged by another card (spell or creature).
        /// </summary>
        /// <param name="damagingCard">The card that damaged this card</param>
        /// <param name="damage">The amount of incoming damage (before mitigation)</param>
        public virtual void OnDamaged(Card damagingCard, int damage)
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

        /// <summary>
        /// Called when this card is destroyed by another card.
        /// </summary>
        /// <param name="destroyer">The card that destroyed this card</param>
        public virtual void OnDestroyed(Card destroyer)
        {
            Destroyed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called before this card attacks a target.
        /// Returns the damage dealt by this card.
        /// </summary>
        /// <param name="target">The card that is being attacked</param>
        public virtual int OnBeforeAttack(Card target)
        {
            return Attack;
        }

        /// <summary>
        /// Called after an attack has been performed.
        /// </summary>
        /// <param name="target">The card that has been attacked</param>
        /// <param name="damage">The damage dealt to the opponent</param>
        public virtual void OnAfterAttack(Card target, int damage)
        {
            return;
        }
        #endregion
    }
}
