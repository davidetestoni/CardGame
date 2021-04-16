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
        public void Start()
        {
            CurrentPlayer = Random.Next() % 2 == 0 ? PlayerOne : PlayerTwo;
            CurrentPlayer.MaximumMana = 1;
            CurrentPlayer.CurrentMana = 1;
            DrawCards(Opponent, Options.InitialHandSize, DrawEventSource.GameStart);
            DrawCards(CurrentPlayer, Options.InitialHandSize, DrawEventSource.GameStart);
            DrawCards(CurrentPlayer, 1, DrawEventSource.TurnStart);
            Status = GameStatus.Started;
        }

        /// <summary>
        /// Plays a creature <paramref name="card"/> from the <paramref name="player"/>'s hand.
        /// </summary>
        public void PlayCreatureFromHand(PlayerInstance player, CreatureCard card)
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

            NotifyAllExceptSelf(instance, c => c.OnCreaturePlayed(player, instance));
        }

        /// <summary>
        /// Ends the <paramref name="player"/>'s turn.
        /// </summary>
        public void EndTurn(PlayerInstance player)
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
        }

        public void AttackCreature(PlayerInstance player, CreatureCardInstance attacker, CreatureCardInstance target)
        {
            CheckTurn(player);

            // If the card has no attacks left
            if (attacker.AttacksLeft == 0)
            {
                throw new Exception("This card cannot attack anymore during this turn");
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
        }

        /// <summary>
        /// Draws <paramref name="count"/> cards from the <paramref name="player"/>'s deck.
        /// </summary>
        /// <param name="drawEventSource">The cause of the draw</param>
        public void DrawCards(PlayerInstance player, int count, DrawEventSource drawEventSource = DrawEventSource.Effect)
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
                    DamagePlayer(player, 1);
                    return;
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
        }

        /// <summary>
        /// Destroys a card on the field and sends it to the graveyard.
        /// </summary>
        public void DestroyCard(CardInstance destroyer, CreatureCardInstance target)
        {
            NotifyAll(c => c.OnCardDestroyed(destroyer, target));
            target.Owner.Field.Remove(target);
            target.Owner.Graveyard.Add(target.Base);
        }

        /// <summary>
        /// Damage the player due to an effect (not for direct attacks).
        /// </summary>
        public void DamagePlayer(PlayerInstance target, int damage)
        {
            if (target.CurrentHealth > damage)
            {
                target.CurrentHealth -= damage;
            }
            else
            {
                target.CurrentHealth = 0;
                CheckVictory();
            }
        }

        /// <summary>
        /// Checks the victory condition and possibly ends the game.
        /// </summary>
        public void CheckVictory()
        {
            // If both players's health drops to 0 at the same time, the player that is currently
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
        }
        #endregion

        #region Private Methods
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
