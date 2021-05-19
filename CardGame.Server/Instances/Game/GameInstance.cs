using CardGame.Server.Enums;
using CardGame.Server.Events.Cards.Creatures;
using CardGame.Server.Events.Game;
using CardGame.Server.Events.Players;
using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame.Server.Instances.Game
{
    /// <summary>
    /// Server-side instance of a game.
    /// </summary>
    public class GameInstance
    {
        #region Public Properties
        /// <summary>
        /// The id of the game.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The first player of the game.
        /// </summary>
        public PlayerInstance PlayerOne { get; set; }

        /// <summary>
        /// The second player of the game.
        /// </summary>
        public PlayerInstance PlayerTwo { get; set; }

        /// <summary>
        /// How many turns have passed since the start of the game.
        /// </summary>
        public int TurnNumber { get; set; } = 1;

        /// <summary>
        /// The player that is currently playing its turn.
        /// </summary>
        public PlayerInstance CurrentPlayer { get; set; }

        /// <summary>
        /// The opponent of the <see cref="CurrentPlayer"/>.
        /// </summary>
        public PlayerInstance Opponent => PlayerOne != CurrentPlayer ? PlayerOne : PlayerTwo;

        /// <summary>
        /// The options of the game.
        /// </summary>
        public GameInstanceOptions Options { get; set; }

        /// <summary>
        /// The random number generator, which is also used by cards e.g. to
        /// pick a random card for an effect that damages a random creature on the enemy field.
        /// </summary>
        public Random Random { get; } = new Random();

        /// <summary>
        /// The status of the game.
        /// </summary>
        public GameStatus Status { get; set; } = GameStatus.Created;

        /// <summary>
        /// The winner of the game, if any.
        /// </summary>
        public PlayerInstance Winner { get; set; } = null;

        /// <summary>
        /// Whether the game finished because of a surrender from either player.
        /// </summary>
        public bool Surrendered { get; set; }
        #endregion

        #region Events
        // Game
        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<NewTurnEventArgs> NewTurn;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        public event EventHandler<CardsDrawnEventArgs> CardsDrawn;
        public event EventHandler<CustomEventArgs> CustomEvent;

        // Player
        public event EventHandler<PlayerAttackedEventArgs> PlayerAttacked;

        // - Mana
        public event EventHandler<PlayerManaRestoredEventArgs> PlayerManaRestored;
        public event EventHandler<PlayerMaxManaIncreasedEventArgs> PlayerMaxManaIncreased;
        public event EventHandler<PlayerManaSpentEventArgs> PlayerManaSpent;

        // - Health
        public event EventHandler<PlayerHealthRestoredEventArgs> PlayerHealthRestored;
        public event EventHandler<PlayerDamagedEventArgs> PlayerDamaged;

        // Creatures
        public event EventHandler<CreaturePlayedEventArgs> CreaturePlayed;
        public event EventHandler<CreatureSpawnedEventArgs> CreatureSpawned;
        public event EventHandler<CreatureAttackedEventArgs> CreatureAttacked;
        public event EventHandler<CreatureDestroyedEventArgs> CreatureDestroyed;
        public event EventHandler<CreatureAttacksLeftChangedEventArgs> CreatureAttacksLeftChanged;

        // - Health
        public event EventHandler<CreatureDamagedEventArgs> CreatureDamaged;
        public event EventHandler<CreatureHealthIncreasedEventArgs> CreatureHealthIncreased;
        
        // - Attack
        public event EventHandler<CreatureAttackChangedEventArgs> CreatureAttackChanged;
        #endregion

        #region Public Methods
        /// <summary>
        /// Randomly selects the current player, draws the initial hands and starts the game.
        /// </summary>
        public GameInstance Start()
        {
            // Randomly select a player who gets to play first
            CurrentPlayer = Random.Next() % 2 == 0 ? PlayerOne : PlayerTwo;
            GameStarted?.Invoke(this, new GameStartedEventArgs { CurrentPlayer = CurrentPlayer });

            // Set max and current mana to 1
            IncreaseMaxMana(CurrentPlayer, 1);
            RestoreMana(CurrentPlayer, 1);

            // Draw cards for both players (1 more for the opponent to reduce the disadvantage of playing second)
            DrawCards(Opponent, Options.InitialHandSize + 1, DrawEventSource.GameStart);
            DrawCards(CurrentPlayer, Options.InitialHandSize, DrawEventSource.GameStart);

            // Draw the card for the current turn
            DrawCards(CurrentPlayer, 1, DrawEventSource.TurnStart);

            Status = GameStatus.Started;
            NewTurn?.Invoke(this, new NewTurnEventArgs { CurrentPlayer = CurrentPlayer, TurnNumber = TurnNumber });

            return this;
        }

        /// <summary>
        /// Plays a <paramref name="creature"/> from the <paramref name="player"/>'s hand.
        /// </summary>
        public GameInstance PlayCreatureFromHand(PlayerInstance player, CreatureCardInstance creature)
        {
            ThrowIfNotPlayerTurn(player);

            if (player.CurrentMana < creature.ManaCost)
            {
                throw new Exception("Not enough mana");
            }

            if (!player.Hand.Contains(creature))
            {
                throw new Exception("The card was not in the player's hand");
            }

            if (player.Field.Count >= Options.FieldSize)
            {
                throw new Exception($"There are already {Options.FieldSize} cards on the field");
            }
                
            // Remove the card from the player's hand and subtract the mana spent
            player.Hand.Remove(creature);
            SpendMana(player, creature.ManaCost);

            // Add it to the field
            player.Field.Add(creature);

            // If the card has Rush, reset its attacks left
            if (creature.Features.HasFlag(CardFeature.Rush))
            {
                ResetAttacksLeft(creature);
            }

            CreaturePlayed?.Invoke(this, new CreaturePlayedEventArgs { Player = player, Creature = creature });
            NotifyAll(c => c.OnCreaturePlayed(player, creature));

            return this;
        }

        /// <summary>
        /// Ends the <paramref name="player"/>'s turn.
        /// </summary>
        public GameInstance EndTurn(PlayerInstance player)
        {
            ThrowIfNotPlayerTurn(player);

            // Proc effects for the turn end
            NotifyAll(c => c.OnTurnEnd(CurrentPlayer, TurnNumber));

            // Set the opponent as the current player
            CurrentPlayer = Opponent;
            TurnNumber++;

            NewTurn?.Invoke(this, new NewTurnEventArgs { CurrentPlayer = CurrentPlayer, TurnNumber = TurnNumber });

            // Gain a new mana point and restore mana
            IncreaseMaxMana(CurrentPlayer, 1);
            RestoreMana(CurrentPlayer, CurrentPlayer.MaximumMana);

            // Draw a card
            DrawCards(CurrentPlayer, 1, DrawEventSource.TurnStart);

            // Reset the attacks left for all allied creatures
            CurrentPlayer.Field.ForEach(c => ResetAttacksLeft(c));

            // Proc effects for the turn start
            NotifyAll(c => c.OnTurnStart(CurrentPlayer, TurnNumber));

            return this;
        }

        /// <summary>
        /// Attacks an enemy creature on the field.
        /// </summary>
        public GameInstance AttackCreature(PlayerInstance player, CreatureCardInstance attacker, CreatureCardInstance defender)
        {
            ThrowIfNotPlayerTurn(player);

            // If the card has no attacks left
            if (attacker.AttacksLeft == 0)
            {
                throw new Exception("This card cannot attack anymore during this turn");
            }

            // If the target has the same owner
            if (attacker.Owner == defender.Owner)
            {
                throw new Exception("The attacker and the target belong to the same player");
            }

            // If there's a card with taunt but I'm not attacking a card with taunt
            if (Opponent.Field.Any(c => c.Features.HasFlag(CardFeature.Taunt)) && !defender.Features.HasFlag(CardFeature.Taunt))
            {
                throw new Exception("There's a card with taunt on the opponent's field");
            }

            NotifyAll(c => c.OnBeforeAttack(attacker, defender));

            SetAttacksLeft(attacker, attacker.AttacksLeft - 1);

            // Calculate the attack damage
            var attackDamage = attacker.GetAttackDamage(defender, false);
            var defenderAttackDamage = defender.GetAttackDamage(attacker, true);
            
            // Calculate the damage taken
            var damage = defender.ComputeDamageTaken(attacker, attackDamage, true);
            var recoilDamage = attacker.ComputeDamageTaken(defender, defenderAttackDamage, false);

            NotifyAll(c => c.OnCardDamaged(attacker, defender, damage));
            NotifyAll(c => c.OnCardDamaged(defender, attacker, recoilDamage));

            CreatureAttacked?.Invoke(this, new CreatureAttackedEventArgs
            {
                Attacker = attacker,
                Defender = defender,
                Damage = damage,
                RecoilDamage = recoilDamage
            });

            DestroyZeroHealth();

            NotifyAll(c => c.OnAfterAttack(attacker, defender, attackDamage));

            return this;
        }

        /// <summary>
        /// Attacks the enemy player directly
        /// </summary>
        public GameInstance AttackPlayer(PlayerInstance player, CreatureCardInstance attacker)
        {
            ThrowIfNotPlayerTurn(player);

            var target = Opponent;

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

            SetAttacksLeft(attacker, attacker.AttacksLeft - 1);

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
                ThrowIfNotPlayerTurn(player);
            }

            List<CardInstance> newCards = new List<CardInstance>();
            List<CardInstance> destroyedCards = new List<CardInstance>();
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
                        newCards.Add(card);
                        player.Hand.Add(card);
                    }
                    // Otherwise send it to the graveyard
                    else
                    {
                        destroyedCards.Add(card);
                        player.Graveyard.Add(card);
                    }
                }

                drawn++;
            }

            NotifyAll(c => c.OnCardsDrawn(player, count, drawEventSource));
            CardsDrawn?.Invoke(this, new CardsDrawnEventArgs 
            {
                Player = player, 
                NewCards = newCards,
                Destroyed = destroyedCards
            });

            return this;
        }

        /// <summary>
        /// Destroys a creature on the field and sends it to the graveyard.
        /// </summary>
        public GameInstance DestroyCreature(CardInstance destroyer, CreatureCardInstance target)
        {
            target.Owner.Field.Remove(target);
            target.Owner.Graveyard.Add(target);

            NotifyAll(c => c.OnCardDestroyed(destroyer, target));

            CreatureDestroyed?.Invoke(this, new CreatureDestroyedEventArgs
            {
                Destroyer = destroyer,
                Target = target,
            });

            return this;
        }

        public GameInstance SetAttacksLeft(CreatureCardInstance creature, int amount)
        {
            creature.AttacksLeft = amount;
            CreatureAttacksLeftChanged?.Invoke(this, new CreatureAttacksLeftChangedEventArgs
            {
                Creature = creature,
                CanAttack = creature.AttacksLeft > 0
            });

            return this;
        }

        public GameInstance ResetAttacksLeft(CreatureCardInstance creature)
        {
            creature.ResetAttacksLeft();
            CreatureAttacksLeftChanged?.Invoke(this, new CreatureAttacksLeftChangedEventArgs 
            {
                Creature = creature, 
                CanAttack = creature.AttacksLeft > 0 
            });

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
            PlayerHealthRestored?.Invoke(this, new PlayerHealthRestoredEventArgs { Player = target, Amount = healAmount });

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
            }

            if (source is CreatureCardInstance creature)
            {
                PlayerAttacked?.Invoke(this, new PlayerAttackedEventArgs { Player = target, Attacker = creature, Damage = damage });
            }
            else
            {
                PlayerDamaged?.Invoke(this, new PlayerDamagedEventArgs { Player = target, Damage = damage });
            }

            CheckVictory();

            return this;
        }

        public GameInstance RestoreMana(PlayerInstance player, int amount)
        {
            var targetMana = Math.Min(player.MaximumMana, player.CurrentMana + amount);
            var increment = targetMana - player.CurrentMana;

            if (increment == 0)
            {
                return this;
            }
            
            player.CurrentMana += increment;

            PlayerManaRestored?.Invoke(this, new PlayerManaRestoredEventArgs { Player = player, Amount = increment });

            return this;
        }

        public GameInstance SpendMana(PlayerInstance player, int amount)
        {
            if (amount > player.CurrentMana)
            {
                throw new Exception("Not enough mana");
            }

            player.CurrentMana -= amount;

            PlayerManaSpent?.Invoke(this, new PlayerManaSpentEventArgs { Player = player, Amount = amount });

            return this;
        }

        public GameInstance IncreaseMaxMana(PlayerInstance player, int amount, bool canOverflow = false)
        {
            var targetMana = player.MaximumMana + amount;

            if (targetMana > Options.MaximumMana && !canOverflow)
            {
                targetMana = Options.MaximumMana;
            }

            var increment = targetMana - player.MaximumMana;

            if (increment == 0)
            {
                return this;
            }

            player.MaximumMana += increment;

            PlayerMaxManaIncreased?.Invoke(this, new PlayerMaxManaIncreasedEventArgs { Player = player, Increment = increment });

            return this;
        }

        /// <summary>
        /// Destroy the cards with zero health.
        /// </summary>
        public GameInstance DestroyZeroHealth()
        {
            for (int i = 0; i < CurrentPlayer.Field.Count; i++)
            {
                var creature = CurrentPlayer.Field[i];

                if (creature.Health == 0)
                {
                    DestroyCreature(null, creature);
                    i--;
                }
            }

            for (int i = 0; i < Opponent.Field.Count; i++)
            {
                var creature = Opponent.Field[i];

                if (creature.Health == 0)
                {
                    DestroyCreature(null, creature);
                    i--;
                }
            }

            return this;
        }

        public GameInstance TriggerCustomEvent(string shortName, object data)
        {
            CustomEvent?.Invoke(this, new CustomEventArgs { ShortName = shortName, Data = data });
            return this;
        }

        public GameInstance Surrender(PlayerInstance player)
        {
            Status = GameStatus.Finished;
            Winner = GetOpponent(player);
            Surrendered = true;
            GameEnded?.Invoke(this, new GameEndedEventArgs { Winner = Winner, Surrendered = true });

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
                GameEnded?.Invoke(this, new GameEndedEventArgs { Winner = Winner, Surrendered = false });
            }
            else if (Opponent.CurrentHealth == 0)
            {
                Status = GameStatus.Finished;
                Winner = CurrentPlayer;
                GameEnded?.Invoke(this, new GameEndedEventArgs { Winner = Winner, Surrendered = false });
            }

            return this;
        }

        public PlayerInstance GetOpponent(PlayerInstance player)
            => player == CurrentPlayer ? Opponent : CurrentPlayer;
        #endregion

        #region Private Methods
        private void ThrowIfNotPlayerTurn(PlayerInstance player)
        {
            if (player != CurrentPlayer)
            {
                throw new Exception("It's not your turn");
            }
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
