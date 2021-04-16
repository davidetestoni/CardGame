using CardGame.Server.Factories;
using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;
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
        /// Randomly selects the current player, draws the initial hand and starts the game.
        /// </summary>
        public void Start()
        {
            CurrentPlayer = Random.Next() % 2 == 0 ? PlayerOne : PlayerTwo;
            DrawCards(CurrentPlayer, Options.InitialHandSize, true);
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
                
            player.Hand.Remove(card);
            player.CurrentMana -= card.ManaCost;

            var instance = (CreatureCardInstance)_cardFactory.Create(card.ShortName, this, player);
            player.Field.Add(instance);

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
            DrawCards(CurrentPlayer, 1, true);

            // Proc effects for the turn start
            NotifyAll(c => c.OnTurnStart(CurrentPlayer, TurnNumber));
        }

        /// <summary>
        /// Draws <paramref name="count"/> cards from the <paramref name="player"/>'s deck.
        /// </summary>
        /// <param name="isInitialDraw">Whether this is the initial draw that happens at the start of a player's turn</param>
        public void DrawCards(PlayerInstance player, int count, bool isInitialDraw)
        {
            int drawn = 0;

            while (drawn < count)
            {
                if (player.Deck.Count == 0)
                {
                    // TODO: When a player runs out of cards, deal damage to the player
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
            }

            NotifyAll(c => c.OnCardsDrawn(player, count, isInitialDraw));
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
