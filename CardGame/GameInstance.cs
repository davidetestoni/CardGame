using CardGame.Models;
using System;

namespace CardGame
{
    public class GameInstance
    {
        #region Public Properties
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public int TurnNumber { get; set; } = 1;
        public Player CurrentPlayer { get; set; }
        public Player Opponent => PlayerOne != CurrentPlayer ? PlayerOne : PlayerTwo;

        public GameInstanceOptions Options { get; set; }

        public bool IsOver { get; set; } = false;
        public Player Winner { get; set; } = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// Plays a creature <paramref name="card"/> from the <paramref name="player"/>'s hand.
        /// </summary>
        public void PlayCreatureFromHand(Player player, CreatureCard card)
        {
            CheckTurn(player);

            if (player.CurrentMana < card.ManaCost)
                throw new Exception("Not enough mana");

            if (!player.Hand.Contains(card))
                throw new Exception("The card was not in the player's hand");

            if (player.Field.Count >= Options.FieldSize)
                throw new Exception($"There are already {Options.FieldSize} cards on the field");

            player.Hand.Remove(card);
            player.CurrentMana -= card.ManaCost;
            player.Field.Add(card);

            Broadcast(c => c.OnCardPlayed(player, card));
        }

        /// <summary>
        /// Plays a spell <paramref name="card"/> from a <paramref name="player"/>'s hand.
        /// </summary>
        public void PlaySpellFromHand(Player player, SpellCard card)
        {
            CheckTurn(player);

            if (player.CurrentMana < card.ManaCost)
                throw new Exception("Not enough mana");

            if (!player.Hand.Contains(card))
                throw new Exception("The card was not in the player's hand");

            player.Hand.Remove(card);
            card.ProcEffect();

            Broadcast(c => c.OnCardPlayed(player, card));
        }

        /// <summary>
        /// Ends the <paramref name="player"/>'s turn.
        /// </summary>
        public void EndTurn(Player player)
        {
            CheckTurn(player);

            // Proc effects for the turn end
            Broadcast(c => c.OnTurnEnd(CurrentPlayer, TurnNumber));

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
            Broadcast(c => c.OnTurnStart(CurrentPlayer, TurnNumber));
        }

        /// <summary>
        /// Draws <paramref name="count"/> cards from the <paramref name="player"/>'s deck.
        /// </summary>
        /// <param name="isInitialDraw">Whether this is the initial draw that happens at the start of a player's turn</param>
        public void DrawCards(Player player, int count, bool isInitialDraw)
        {
            int drawn = 0;
            while (drawn < count)
            {
                // TODO: Handle case when a player runs out of cards in the deck

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

            Broadcast(c => c.OnCardsDrawn(player, count, isInitialDraw));
        }
        #endregion

        #region Private Methods
        private void CheckTurn(Player player)
        {
            if (player != CurrentPlayer)
                throw new Exception("It's not your turn");
        }

        private void Broadcast(Action<CreatureCard> action)
        {
            CurrentPlayer.Field.ForEach(c => action(c));
            Opponent.Field.ForEach(c => action(c));
        }
        #endregion
    }
}
