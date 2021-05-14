using CardGame.Server.Factories;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Models.Cards;
using SampleGame.Cards.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleGame.Tests.Extensions
{
    public static class GameInstanceExtensions
    {
        /// <summary>
        /// Helper method that sets the current and maximum mana values of players.
        /// </summary>
        public static GameInstance SetMana(this GameInstance game, int currentPlayerMana, int opponentMana)
        {
            game.CurrentPlayer.CurrentMana = currentPlayerMana;
            game.CurrentPlayer.MaximumMana = currentPlayerMana;

            game.Opponent.CurrentMana = opponentMana;
            game.Opponent.MaximumMana = opponentMana;

            return game;
        }

        /// <summary>
        /// Helper method that sets the current hands of players.
        /// </summary>
        public static GameInstance SetHands(this GameInstance game, Card currentPlayerCard, Card opponentCard = null)
            => SetHands(game, new List<Card> { currentPlayerCard }, opponentCard is null ? new List<Card>() : new List<Card> { opponentCard });

        /// <summary>
        /// Helper method that sets the current hands of players.
        /// </summary>
        public static GameInstance SetHands(this GameInstance game, List<Card> currentPlayerHand = null, List<Card> opponentHand = null)
        {
            currentPlayerHand ??= new();
            opponentHand ??= new();

            if (currentPlayerHand.Count > game.Options.MaximumHandSize)
            {
                throw new ArgumentException("Too many cards in the current player's hand");
            }

            if (opponentHand.Count > game.Options.MaximumHandSize)
            {
                throw new ArgumentException("Too many cards in the opponent's hand");
            }

            game.CurrentPlayer.Hand = currentPlayerHand;
            game.Opponent.Hand = opponentHand;

            return game;
        }

        /// <summary>
        /// Helper method that sets a 1 vs 1 scenario.
        /// </summary>
        public static GameInstance SetFields1v1(this GameInstance game, Card currentPlayerCard, Card opponentCard = null)
            => SetFields(game,
                currentPlayerCard is null ? new List<Card>() : new List<Card> { currentPlayerCard },
                opponentCard is null ? new List<Card>() : new List<Card> { opponentCard });

        /// <summary>
        /// Helper method that sets a 1 vs many scenario.
        /// </summary>
        public static GameInstance SetFields1vMany(this GameInstance game, Card currentPlayerCard, List<Card> opponentField = null)
            => SetFields(game,
                currentPlayerCard is null ? new List<Card>() : new List<Card> { currentPlayerCard },
                opponentField);

        /// <summary>
        /// Helper method that sets the current fields of players.
        /// </summary>
        public static GameInstance SetFields(this GameInstance game, List<Card> currentPlayerField, List<Card> opponentField = null)
        {
            currentPlayerField ??= new();
            opponentField ??= new();

            if (currentPlayerField.Count > game.Options.FieldSize)
            {
                throw new ArgumentException("Too many cards in the current player's field");
            }

            if (opponentField.Count > game.Options.FieldSize)
            {
                throw new ArgumentException("Too many cards in the opponent's field");
            }

            var factory = new CardInstanceFactory(typeof(BasicSoldier).Assembly);
            game.CurrentPlayer.Field.Clear();
            game.Opponent.Field.Clear();

            foreach (var card in currentPlayerField)
            {
                var instance = factory.Create(card.ShortName, game, game.CurrentPlayer) as CreatureCardInstance;
                game.CurrentPlayer.Field.Add(instance);
            }

            foreach (var card in opponentField)
            {
                var instance = factory.Create(card.ShortName, game, game.Opponent) as CreatureCardInstance;
                game.Opponent.Field.Add(instance);
            }

            return game;
        }

        /// <summary>
        /// Helper method that resets the attacks left of all the cards on the
        /// current player's field.
        /// </summary>
        public static GameInstance ResetAttacksLeft(this GameInstance game)
        {
            game.CurrentPlayer.Field.ForEach(c => c.ResetAttacksLeft());
            return game;
        }

        /// <summary>
        /// Helper method that end the current player's turn.
        /// </summary>
        public static GameInstance EndCurrentTurn(this GameInstance game)
        {
            game.EndTurn(game.CurrentPlayer);
            return game;
        }
    }
}
