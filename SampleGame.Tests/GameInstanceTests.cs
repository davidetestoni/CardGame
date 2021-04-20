﻿using CardGame.Server.Enums;
using CardGame.Server.Instances.Game;
using CardGame.Shared.Models.Cards;
using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SampleGame.Tests
{
    public class GameInstanceTests : IClassFixture<FactoryFixture>
    {
        private readonly FactoryFixture _factoryFixture;

        public GameInstanceTests(FactoryFixture factoryFixture)
        {
            _factoryFixture = factoryFixture;
        }

        #region Start
        [Fact]
        public void Start_StandardSettings_GameStarted()
        {
            var game = _factoryFixture.CreateTestGame();
            Assert.Equal(GameStatus.Created, game.Status);

            game.Start();
            Assert.Equal(GameStatus.Started, game.Status);
            Assert.Equal(1, game.TurnNumber);
            Assert.NotNull(game.PlayerOne);
            Assert.NotNull(game.PlayerTwo);
            Assert.NotNull(game.CurrentPlayer);
            Assert.True(game.CurrentPlayer == game.PlayerOne || game.CurrentPlayer == game.PlayerTwo);
            Assert.Null(game.Winner);
            Assert.Equal(1, game.CurrentPlayer.CurrentMana);
            Assert.Equal(0, game.Opponent.CurrentMana);
        }

        [Fact]
        public void Start_StandardSettings_HandsDrawn()
        {
            var game = _factoryFixture.CreateTestGame();
            var player1DeckSize = game.PlayerOne.Deck.Count;
            var player2DeckSize = game.PlayerTwo.Deck.Count;

            game.Start();

            var player1HandSize = game.CurrentPlayer == game.PlayerOne
                ? game.Options.InitialHandSize + 1
                : game.Options.InitialHandSize;

            var player2HandSize = game.CurrentPlayer == game.PlayerTwo
                ? game.Options.InitialHandSize + 1
                : game.Options.InitialHandSize;

            Assert.Equal(player1HandSize, game.PlayerOne.Hand.Count);
            Assert.Equal(player2HandSize, game.PlayerTwo.Hand.Count);

            Assert.Equal(player1HandSize, player1DeckSize - game.PlayerOne.Deck.Count);
            Assert.Equal(player2HandSize, player2DeckSize - game.PlayerTwo.Deck.Count);
        }
        #endregion

        #region PlayCreatureFromHand
        [Fact]
        public void PlayCreatureFromHand_EnoughMana_Summon()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var handSize = game.CurrentPlayer.Hand.Count;
            var card = (CreatureCard)game.CurrentPlayer.Hand[0];
            game.PlayCreatureFromHand(game.CurrentPlayer, card);

            Assert.Equal(handSize - 1, game.CurrentPlayer.Hand.Count);
            Assert.Single(game.CurrentPlayer.Field);
            Assert.Equal(0, game.CurrentPlayer.CurrentMana);
        }

        [Fact]
        public void PlayCreatureFromHand_NotEnoughMana_Throws()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var card1 = (CreatureCard)game.CurrentPlayer.Hand[0];
            var card2 = (CreatureCard)game.CurrentPlayer.Hand[1];
            game.PlayCreatureFromHand(game.CurrentPlayer, card1);
            
            Assert.Throws<Exception>(() => game.PlayCreatureFromHand(game.CurrentPlayer, card2));
        }

        [Fact]
        public void PlayCreatureFromHand_WrongPlayer_Throws()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var card = (CreatureCard)game.CurrentPlayer.Hand[0];
            Assert.Throws<Exception>(() => game.PlayCreatureFromHand(game.Opponent, card));
        }
        #endregion

        #region EndTurn
        [Fact]
        public void EndTurn_InTime_ChangePlayer()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var opponent = game.Opponent;
            var opponentMaxMana = game.Opponent.MaximumMana;
            var opponentHandSize = game.Opponent.Hand.Count;

            game.EndTurn(game.CurrentPlayer);

            Assert.Same(opponent, game.CurrentPlayer);
            Assert.Equal(opponentMaxMana + 1, game.CurrentPlayer.MaximumMana);
            Assert.Equal(opponentMaxMana + 1, game.CurrentPlayer.CurrentMana);
            Assert.Equal(opponentHandSize + 1, game.CurrentPlayer.Hand.Count);
        }

        [Fact]
        public void EndTurn_WrongPlayer_Throws()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            Assert.Throws<Exception>(() => game.EndTurn(game.Opponent));
        }
        #endregion

        #region DrawCards
        [Fact]
        public void DrawCards_Effect_Draws()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var handSize = game.CurrentPlayer.Hand.Count;
            game.DrawCards(game.CurrentPlayer, 1, DrawEventSource.Effect);
            Assert.Equal(handSize + 1, game.CurrentPlayer.Hand.Count);
        }

        [Fact]
        public void DrawCards_Multiple_Draws()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var handSize = game.CurrentPlayer.Hand.Count;
            game.DrawCards(game.CurrentPlayer, 2, DrawEventSource.Effect);
            Assert.Equal(handSize + 2, game.CurrentPlayer.Hand.Count);
        }

        [Fact]
        public void DrawCards_HandFull_DestroysExtras()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var handRoom = game.Options.MaximumHandSize - game.CurrentPlayer.Hand.Count;
            game.DrawCards(game.CurrentPlayer, handRoom + 1, DrawEventSource.Effect);
            Assert.Equal(game.Options.MaximumHandSize, game.CurrentPlayer.Hand.Count);
            Assert.Single(game.CurrentPlayer.Graveyard);
        }

        [Fact]
        public void DrawCards_DeckEmpty_DamagePlayer()
        {
            var game = _factoryFixture.CreateTestGame(6);
            game.Start();

            game.DrawCards(game.CurrentPlayer, game.CurrentPlayer.Deck.Count + 1, DrawEventSource.Effect);
            Assert.Equal(game.CurrentPlayer.InitialHealth - 1, game.CurrentPlayer.CurrentHealth);
        }
        #endregion

        #region AttackCreature
        [Fact]
        public void AttackCreature_SoldierSoldier_BothDestroyed()
        {
            var game = 
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new BasicSoldier(), new BasicSoldier())
                .ResetAttacksLeft();

            // Attack the opponent's basic soldier
            game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], game.Opponent.Field[0]);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);
        }

        [Fact]
        public void AttackCreature_SoldierGunner_SoldierDestroyedGunnerDamaged()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new BasicSoldier(), new Gunner())
                .ResetAttacksLeft();

            // Attack the gunner with the soldier
            var soldier = game.CurrentPlayer.GetCreatureOnField<BasicSoldier>();
            var gunner = game.Opponent.GetCreatureOnField<Gunner>();
            game.AttackCreature(game.CurrentPlayer, soldier, gunner);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Single(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Empty(game.Opponent.Graveyard);

            Assert.Equal(gunner.Base.Health - 1, gunner.Health);
        }

        [Fact]
        public void AttackCreature_GunnerSoldier_GunnerDamagedSoldierDestroyed()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new Gunner(), new BasicSoldier())
                .ResetAttacksLeft();

            // Attack the soldier with the gunner
            var gunner = game.CurrentPlayer.GetCreatureOnField<Gunner>();
            var soldier = game.Opponent.GetCreatureOnField<BasicSoldier>();
            game.AttackCreature(game.CurrentPlayer, gunner, soldier);

            Assert.Single(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Empty(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);

            Assert.Equal(gunner.Base.Health - 1, gunner.Health);
            Assert.Equal(0, gunner.AttacksLeft);
        }

        [Fact]
        public void AttackCreature_NoAttacksLeft_Throws()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new BasicSoldier(), new BasicSoldier());

            // Try to attack the enemy soldier
            var mySoldier = game.CurrentPlayer.GetCreatureOnField<BasicSoldier>();
            var enemySoldier = game.Opponent.GetCreatureOnField<BasicSoldier>();
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, mySoldier, enemySoldier));
        }

        [Fact]
        public void AttackCreature_OwnField_Throws()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new List<Card> { new BasicSoldier(), new Gunner() })
                .ResetAttacksLeft();

            // Try to attack the gunner with the soldier
            var soldier = game.CurrentPlayer.GetCreatureOnField<BasicSoldier>();
            var gunner = game.CurrentPlayer.GetCreatureOnField<Gunner>();
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, soldier, gunner));
        }
        #endregion

        // ----------
        //  FEATURES
        // ----------

        #region Taunt
        [Fact]
        public void AttackCreature_Taunt_AttackOther_Throws()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new BasicSoldier(), new List<Card> { new Defender(), new BasicSoldier() })
                .ResetAttacksLeft();

            // Try to attack the soldier
            var mySoldier = game.CurrentPlayer.GetCreatureOnField<BasicSoldier>();
            var enemySoldier = game.Opponent.GetCreatureOnField<BasicSoldier>();
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, mySoldier, enemySoldier));
        }

        [Fact]
        public void AttackCreature_Taunt_AttackTaunter_Ok()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new BasicSoldier(), new List<Card> { new Defender(), new BasicSoldier() })
                .ResetAttacksLeft();

            // Attack the defender
            var mySoldier = game.CurrentPlayer.GetCreatureOnField<BasicSoldier>();
            var defender = game.Opponent.GetCreatureOnField<Defender>();
            game.AttackCreature(game.CurrentPlayer, mySoldier, defender);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Equal(2, game.Opponent.Field.Count);

            Assert.Equal(defender.Base.Health - 1, defender.Health);
        }
        #endregion

        #region Rush
        [Fact]
        public void AttackCreature_Rush_AttackImmediately()
        {
            var game = 
                _factoryFixture.CreateTestGame()
                .Start()
                .SetMana(2, 2)
                .SetHands(new Quickshot())
                .SetFields(null, new BasicSoldier());

            // Play the quickshot
            var quickshotCard = game.CurrentPlayer.GetCreatureInHand<Quickshot>();
            game.PlayCreatureFromHand(game.CurrentPlayer, quickshotCard);

            // Attack immediately
            var quickshot = game.CurrentPlayer.GetCreatureOnField<Quickshot>();
            var soldier = game.Opponent.GetCreatureOnField<BasicSoldier>();
            game.AttackCreature(game.CurrentPlayer, quickshot, soldier);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);
        }
        #endregion
    }
}
