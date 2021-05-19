using CardGame.Server.Enums;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Enums;
using CardGame.Shared.Models.Cards;
using SampleGame.Cards.Creatures;
using SampleGame.Tests.Extensions;
using System;
using System.Collections.Generic;
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

            var player1HandSize = game.Options.InitialHandSize + 1;
            var player2HandSize = game.Options.InitialHandSize + 1;

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
            var card = (CreatureCardInstance)game.CurrentPlayer.Hand[0];
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

            var card1 = (CreatureCardInstance)game.CurrentPlayer.Hand[0];
            var card2 = (CreatureCardInstance)game.CurrentPlayer.Hand[1];
            game.PlayCreatureFromHand(game.CurrentPlayer, card1);
            
            Assert.Throws<Exception>(() => game.PlayCreatureFromHand(game.CurrentPlayer, card2));
        }

        [Fact]
        public void PlayCreatureFromHand_WrongPlayer_Throws()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var card = (CreatureCardInstance)game.CurrentPlayer.Hand[0];
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
        public void AttackCreature_RolandRoland_BothDestroyed()
        {
            var game = 
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Roland(), new Roland())
                .ResetAttacksLeft();

            // Attack the opponent's Roland
            game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], game.Opponent.Field[0]);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);
        }

        [Fact]
        public void AttackCreature_RolandSimon_RolandDestroyedSimonDamaged()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Roland(), new Simon())
                .ResetAttacksLeft();

            // Attack Simon with Roland
            var roland = game.CurrentPlayer.GetCreatureOnField<Roland>();
            var simon = game.Opponent.GetCreatureOnField<Simon>();
            game.AttackCreature(game.CurrentPlayer, roland, simon);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Single(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Empty(game.Opponent.Graveyard);

            Assert.Equal(simon.Base.Health - 1, simon.Health);
        }

        [Fact]
        public void AttackCreature_SimonRoland_SimonDamagedRolandDestroyed()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Simon(), new Roland())
                .ResetAttacksLeft();

            // Attack Roland with Simon
            var simon = game.CurrentPlayer.GetCreatureOnField<Simon>();
            var roland = game.Opponent.GetCreatureOnField<Roland>();
            game.AttackCreature(game.CurrentPlayer, simon, roland);

            Assert.Single(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Empty(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);

            Assert.Equal(simon.Base.Health - 1, simon.Health);
            Assert.Equal(0, simon.AttacksLeft);
        }

        [Fact]
        public void AttackCreature_NoAttacksLeft_Throws()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Roland(), new Roland());

            // Try to attack the enemy Roland
            var myRoland = game.CurrentPlayer.GetCreatureOnField<Roland>();
            var enemyRoland = game.Opponent.GetCreatureOnField<Roland>();
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, myRoland, enemyRoland));
        }

        [Fact]
        public void AttackCreature_OwnField_Throws()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields(new List<Card> { new Roland(), new Simon() })
                .ResetAttacksLeft();

            // Try to attack Simon with Roland
            var roland = game.CurrentPlayer.GetCreatureOnField<Roland>();
            var simon = game.CurrentPlayer.GetCreatureOnField<Simon>();
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, roland, simon));
        }
        #endregion

        #region AttackPlayer
        [Fact]
        public void AttackPlayer_Roland_DealsDamage()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Roland(), null)
                .ResetAttacksLeft();

            // Attack the opponent
            var roland = game.CurrentPlayer.GetCreatureOnField<Roland>();
            game.AttackPlayer(game.CurrentPlayer, roland);

            Assert.Equal(game.Opponent.InitialHealth - roland.Attack, game.Opponent.CurrentHealth);
            Assert.Equal(0, roland.AttacksLeft);
            Assert.Equal(roland.Base.Health, roland.Health);
        }
        #endregion

        #region RestoreCreatureHealth
        [Fact]
        public void RestoreCreatureHealth_LessThanFull_RestoreAmount()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Simon());

            var simon = game.CurrentPlayer.GetCreatureOnField<Simon>();
            simon.Health = 1;
            game.RestoreCreatureHealth(simon, 2);
            Assert.Equal(3, simon.Health);
        }

        [Fact]
        public void RestoreCreatureHealth_MoreThanFull_BringToFull()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1v1(new Simon());

            var simon = game.CurrentPlayer.GetCreatureOnField<Simon>();
            simon.Health = 2;
            game.RestoreCreatureHealth(simon, 2);
            Assert.Equal(3, simon.Health);
        }
        #endregion

        #region RestorePlayerHealth
        [Fact]
        public void RestorePlayerHealth_LessThanFull_RestoreAmount()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start();

            game.CurrentPlayer.CurrentHealth -= 2;
            game.RestorePlayerHealth(game.CurrentPlayer, 2);
            Assert.Equal(game.CurrentPlayer.InitialHealth, game.CurrentPlayer.CurrentHealth);
        }

        [Fact]
        public void RestorePlayerHealth_MoreThanFull_BringToFull()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start();

            game.CurrentPlayer.CurrentHealth -= 2;
            game.RestorePlayerHealth(game.CurrentPlayer, 3);
            Assert.Equal(game.CurrentPlayer.InitialHealth, game.CurrentPlayer.CurrentHealth);
        }
        #endregion

        #region Surrender
        [Fact]
        public void Surrender_CurrentPlayer_GameEnds()
        {
            var game = _factoryFixture.CreateTestGame();
            game.Start();

            var currentPlayer = game.CurrentPlayer;
            var opponent = game.Opponent;
            game.Surrender(currentPlayer);

            Assert.Equal(GameStatus.Finished, game.Status);
            Assert.Equal(opponent, game.Winner);
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
                .SetFields(new List<Card> { new Roland() }, new List<Card> { new Lenny(), new Roland() })
                .ResetAttacksLeft();

            // Try to attack the enemy Roland
            var myRoland = game.CurrentPlayer.GetCreatureOnField<Roland>();
            var enemyRoland = game.Opponent.GetCreatureOnField<Roland>();
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, myRoland, enemyRoland));
        }

        [Fact]
        public void AttackCreature_Taunt_AttackTaunter_Ok()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetFields1vMany(new Roland(), new List<Card> { new Lenny(), new Roland() })
                .ResetAttacksLeft();

            // Attack Lenny with Roland
            var roland = game.CurrentPlayer.GetCreatureOnField<Roland>();
            var lenny = game.Opponent.GetCreatureOnField<Lenny>();
            game.AttackCreature(game.CurrentPlayer, roland, lenny);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Equal(2, game.Opponent.Field.Count);

            Assert.Equal(lenny.Base.Health - 1, lenny.Health);
        }
        #endregion

        #region Rush
        [Fact]
        public void AttackCreature_Rush_AttackImmediately()
        {
            var game =
                _factoryFixture.CreateTestGame()
                .Start()
                .SetMana(2, 2);

            var evie = _factoryFixture.CardFactory.Create<Evie>(game, game.CurrentPlayer) as CreatureCardInstance;

            game
                .SetHands(evie)
                .SetFields1v1(null, new Roland());

            // Play Evie
            game.PlayCreatureFromHand(game.CurrentPlayer, evie);

            // Attack immediately
            var roland = game.Opponent.GetCreatureOnField<Roland>();
            game.AttackCreature(game.CurrentPlayer, evie, roland);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);
        }
        #endregion
    }
}
