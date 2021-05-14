using CardGame.Server.Enums;
using CardGame.Server.Factories;
using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Enums;
using CardGame.Shared.Models.Cards;
using System;
using System.Linq;

namespace CardGame.Server.Instances.Game
{
    public class GameInstance
    {
        private readonly CardInstanceFactory _cardFactory;

        #region Public Properties
        public PlayerInstance PlayerOne { get; set; }
        public PlayerInstance PlayerTwo { get; set; }

        public int TurnNumber { get; set; } = 1;
        public PlayerInstance CurrentPlayer { get; set; }
        public PlayerInstance Opponent => PlayerOne != CurrentPlayer ? PlayerOne : PlayerTwo;

        public GameInstanceOptions Options { get; set; }
        public Random Random { get; } = new();

        public GameStatus Status { get; set; } = GameStatus.Created;
        public PlayerInstance Winner { get; set; } = null;
        #endregion

        /// <summary>
        /// Creates a <see cref="GameInstance"/> given a <paramref name="cardFactory"/>.
        /// </summary>
        /// <param name="cardFactory"></param>
        public GameInstance(CardInstanceFactory cardFactory)
        {
            _cardFactory = cardFactory;
        }

        #region Public Methods
        /// <summary>
        /// Randomly selects the current player, draws the initial hands and starts the game.
        /// </summary>
        public GameInstance Start()
        {
            CurrentPlayer = Random.Next() % 2 == 0 ? PlayerOne : PlayerTwo;
            CurrentPlayer.MaximumMana = 1;
            CurrentPlayer.CurrentMana = 1;
            DrawCards(Opponent, Options.InitialHandSize, DrawEventSource.GameStart);
            DrawCards(CurrentPlayer, Options.InitialHandSize, DrawEventSource.GameStart);
            DrawCards(CurrentPlayer, 1, DrawEventSource.TurnStart);
            Status = GameStatus.Started;

            return this;
        }

        /// <summary>
        /// Plays a creature <paramref name="card"/> from the <paramref name="player"/>'s hand.
        /// </summary>
        public GameInstance PlayCreatureFromHand(PlayerInstance player, CreatureCard card)
        {
            CheckTurn(player);

            if (player.CurrentMana < card.ManaCost)
            {
                throw new Exception("Not enough mana");
            }

            if (!player.Hand.Contains(card))
            {
                throw new Exception("The card was not in the player's hand");
            }

            if (player.Field.Count >= Options.FieldSize)
            {
                throw new Exception($"There are already {Options.FieldSize} cards on the field");
            }
                
            // Remove the card from the player's hand and subtract the mana spent
            player.Hand.Remove(card);
            player.CurrentMana -= card.ManaCost;

            // Create an instance of the card and add it to the field
            var instance = (CreatureCardInstance)_cardFactory.Create(card.ShortName, this, player);
            player.Field.Add(instance);

            // If the card has Rush, reset its attacks left
            if (instance.Features.HasFlag(CardFeature.Rush))
            {
                instance.ResetAttacksLeft();
            }

            NotifyAll(c => c.OnCreaturePlayed(player, instance));

            return this;
        }

        /// <summary>
        /// Ends the <paramref name="player"/>'s turn.
        /// </summary>
        public GameInstance EndTurn(PlayerInstance player)
        {
            CheckTurn(player);

            // Proc effects for the turn end
            NotifyAll(c => c.OnTurnEnd(CurrentPlayer, TurnNumber));

            // Set the opponent as the current player
            CurrentPlayer = Opponent;
            TurnNumber++;
            
            // Gain a new mana point
            if (CurrentPlayer.MaximumMana < Options.MaximumMana)
            {
                CurrentPlayer.MaximumMana++;
            }

            // Restore the maximum mana
            CurrentPlayer.CurrentMana = CurrentPlayer.MaximumMana;

            // Draw a card
            DrawCards(CurrentPlayer, 1, DrawEventSource.TurnStart);

            // Reset the attacks left for all allied creatures
            CurrentPlayer.Field.ForEach(c => c.ResetAttacksLeft());

            // Proc effects for the turn start
            NotifyAll(c => c.OnTurnStart(CurrentPlayer, TurnNumber));

            return this;
        }

        /// <summary>
        /// Attacks an enemy creature on the field.
        /// </summary>
        public GameInstance AttackCreature(PlayerInstance player, CreatureCardInstance attacker, CreatureCardInstance target)
        {
            CheckTurn(player);

            // If the card has no attacks left
            if (attacker.AttacksLeft == 0)
            {
                throw new Exception("This card cannot attack anymore during this turn");
            }

            // If the target has the same owner
            if (attacker.Owner == target.Owner)
            {
                throw new Exception("The attacker and the target belong to the same player");
            }

            // If there's a card with taunt but I'm not attacking a card with taunt
            if (Opponent.Field.Any(c => c.Features.HasFlag(CardFeature.Taunt)) && !target.Features.HasFlag(CardFeature.Taunt))
            {
                throw new Exception("There's a card with taunt on the opponent's field");
            }

            NotifyAll(c => c.OnBeforeAttack(attacker, target));

            attacker.AttacksLeft--;

            var damage = attacker.GetAttackDamage(target);
            var recoilDamage = target.GetAttackDamage(target);

            NotifyAll(c => c.OnCardDamaged(attacker, target, damage));
            NotifyAll(c => c.OnCardDamaged(target, attacker, recoilDamage));

            DestroyZeroHealth();

            NotifyAll(c => c.OnAfterAttack(attacker, target, damage));

            return this;
        }

        /// <summary>
        /// Attacks the enemy player directly
        /// </summary>
        public GameInstance AttackPlayer(PlayerInstance player, CreatureCardInstance attacker, PlayerInstance target)
        {
            CheckTurn(player);

            // If the card has no attacks left
            if (attacker.AttacksLeft == 0)
            {
                throw new Exception("This card cannot attack anymore during this turn");
            }

            // If the target is the player
            if (attacker.Owner == target)
            {
                throw new Exception("You cannot attack yourself");
            }

            // If there's a card with taunt but I'm not attacking a card with taunt
            if (Opponent.Field.Any(c => c.Features.HasFlag(CardFeature.Taunt)))
            {
                throw new Exception("There's a card with taunt on the opponent's field");
            }

            NotifyAll(c => c.OnBeforeAttack(attacker, target));

            attacker.AttacksLeft--;

            var damage = attacker.GetAttackDamage(target);
            DamagePlayer(attacker, target, damage);

            DestroyZeroHealth();

            NotifyAll(c => c.OnAfterAttack(attacker, target, damage));

            return this;
        }

        /// <summary>
        /// Draws <paramref name="count"/> cards from the <paramref name="player"/>'s deck.
        /// </summary>
        /// <param name="drawEventSource">The cause of the draw</param>
        public GameInstance DrawCards(PlayerInstance player, int count, DrawEventSource drawEventSource = DrawEventSource.Effect)
        {
            if (drawEventSource != DrawEventSource.GameStart)
            {
                CheckTurn(player);
            }

            int drawn = 0;

            while (drawn < count)
            {
                if (player.Deck.Count == 0)
                {
                    DamagePlayer(null, player, 1);
                    return this;
                }
                else
                {
                    // Remove the card from the deck
                    var card = player.Deck[0];
                    player.Deck.RemoveAt(0);

                    // If it fits, put it in hand
                    if (player.Hand.Count < Options.MaximumHandSize)
                    {
                        player.Hand.Add(card);
                    }
                    // Otherwise send it to the graveyard
                    else
                    {
                        player.Graveyard.Add(card);
                    }
                }

                drawn++;
            }

            NotifyAll(c => c.OnCardsDrawn(player, count, drawEventSource));

            return this;
        }

        /// <summary>
        /// Destroys a card on the field and sends it to the graveyard.
        /// </summary>
        public GameInstance DestroyCard(CardInstance destroyer, CreatureCardInstance target)
        {
            target.Owner.Field.Remove(target);
            target.Owner.Graveyard.Add(target.Base);

            NotifyAll(c => c.OnCardDestroyed(destroyer, target));

            return this;
        }

        /// <summary>
        /// Restores health to the target, capped at the maximum health value.
        /// </summary>
        public GameInstance RestoreCreatureHealth(CreatureCardInstance target, int amount)
        {
            var healAmount = Math.Min(target.Base.Health - target.Health, amount);
            target.Health += healAmount;

            NotifyAll(c => c.OnCreatureHealed(target, healAmount));

            return this;
        }

        public GameInstance RestorePlayerHealth(PlayerInstance target, int amount)
        {
            var healAmount = Math.Min(target.InitialHealth - target.CurrentHealth, amount);
            target.CurrentHealth += healAmount;

            NotifyAll(c => c.OnPlayerHealed(target, healAmount));

            return this;
        }

        /// <summary>
        /// Damage the player due to an effect (not for direct attacks).
        /// </summary>
        public GameInstance DamagePlayer(CardInstance source, PlayerInstance target, int damage)
        {
            if (target.CurrentHealth > damage)
            {
                target.CurrentHealth -= damage;
                NotifyAll(c => c.OnPlayerDamaged(source, target, damage));
            }
            else
            {
                target.CurrentHealth = 0;
                CheckVictory();
            }

            return this;
        }

        /// <summary>
        /// Destroy the cards with zero health.
        /// </summary>
        public GameInstance DestroyZeroHealth()
        {
            for (int i = 0; i < CurrentPlayer.Field.Count; i++)
            {
                var card = CurrentPlayer.Field[i];

                if (card.Health == 0)
                {
                    card.Owner.Field.Remove(card);
                    card.Owner.Graveyard.Add(card.Base);
                    i--;
                }
            }

            for (int i = 0; i < Opponent.Field.Count; i++)
            {
                var card = Opponent.Field[i];

                if (card.Health == 0)
                {
                    card.Owner.Field.Remove(card);
                    card.Owner.Graveyard.Add(card.Base);
                    i--;
                }
            }

            return this;
        }

        public GameInstance Surrender(PlayerInstance player)
        {
            Status = GameStatus.Finished;
            Winner = GetOpponent(player);

            return this;
        }

        /// <summary>
        /// Checks the victory condition and possibly ends the game.
        /// </summary>
        public GameInstance CheckVictory()
        {
            // If both players' health drops to 0 at the same time, the player that is currently
            // playing will lose.
            if (CurrentPlayer.CurrentHealth == 0)
            {
                Status = GameStatus.Finished;
                Winner = Opponent;
            }
            else if (Opponent.CurrentHealth == 0)
            {
                Status = GameStatus.Finished;
                Winner = CurrentPlayer;
            }

            return this;
        }
        #endregion

        #region Private Methods
        private PlayerInstance GetOpponent(PlayerInstance player)
            => player == CurrentPlayer ? Opponent : CurrentPlayer;

        private void CheckTurn(PlayerInstance player)
        {
            if (player != CurrentPlayer)
                throw new Exception("It's not your turn");
        }

        private void NotifyAll(Action<CreatureCardInstance> action)
        {
            // Clone the player's field at the time of the notification
            var currentField = CurrentPlayer.Field.ToList();

            foreach (var card in currentField)
            {
                // If the card is still on the field
                if (CurrentPlayer.Field.Contains(card))
                {
                    action.Invoke(card);
                }
            }

            // Clone the player's field at the time of the notification
            var opponentField = Opponent.Field.ToList();

            foreach (var card in opponentField)
            {
                // If the card is still on the field
                if (Opponent.Field.Contains(card))
                {
                    action.Invoke(card);
                }
            }
        }

        private void NotifyAllExceptSelf(CreatureCardInstance self, Action<CreatureCardInstance> action)
        {
            // Clone the player's field at the time of the notification
            var currentField = CurrentPlayer.Field.ToList();

            foreach (var card in currentField)
            {
                // If the card is still on the field
                if (CurrentPlayer.Field.Contains(card) && card != self)
                {
                    action.Invoke(card);
                }
            }

            // Clone the player's field at the time of the notification
            var opponentField = Opponent.Field.ToList();

            foreach (var card in opponentField)
            {
                // If the card is still on the field
                if (Opponent.Field.Contains(card) && card != self)
                {
                    action.Invoke(card);
                }
            }
        }
        #endregion
    }
}
