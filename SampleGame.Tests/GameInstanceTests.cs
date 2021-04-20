using CardGame.Server.Enums;
using CardGame.Server.Factories;
using CardGame.Server.Instances.Game;
using CardGame.Shared.Models.Cards;
using CardGame.Shared.Models.Players;
using SampleGame.Cards.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SampleGame.Tests
{
    public class FactoryFixture
    {
        public CardInstanceFactory CardFactory { get; private set; }
        public PlayerInstanceFactory PlayerFactory { get; private set; }
        public GameInstanceFactory GameFactory { get; private set; }

        public FactoryFixture()
        {
            CardFactory = new(typeof(BasicSoldier).Assembly);
            PlayerFactory = new();
            GameFactory = new(CardFactory, PlayerFactory);
        }
    }

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
            var game = CreateTestGame();
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
            var game = CreateTestGame();
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
            var game = CreateTestGame();
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
            var game = CreateTestGame();
            game.Start();

            var card1 = (CreatureCard)game.CurrentPlayer.Hand[0];
            var card2 = (CreatureCard)game.CurrentPlayer.Hand[1];
            game.PlayCreatureFromHand(game.CurrentPlayer, card1);
            
            Assert.Throws<Exception>(() => game.PlayCreatureFromHand(game.CurrentPlayer, card2));
        }

        [Fact]
        public void PlayCreatureFromHand_WrongPlayer_Throws()
        {
            var game = CreateTestGame();
            game.Start();

            var card = (CreatureCard)game.CurrentPlayer.Hand[0];
            Assert.Throws<Exception>(() => game.PlayCreatureFromHand(game.Opponent, card));
        }

        [Fact]
        public void PlayCreatureFromHand_Effect_Procs()
        {
            var game = CreateTestGame();
            game.Start();

            // 1) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 2) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 1) Play 1 booster
            game.CurrentPlayer.Hand = new List<Card> { new Booster() };
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);

            Assert.Single(game.CurrentPlayer.Hand);
        }
        #endregion

        #region EndTurn
        [Fact]
        public void EndTurn_InTime_ChangePlayer()
        {
            var game = CreateTestGame();
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
            var game = CreateTestGame();
            game.Start();

            Assert.Throws<Exception>(() => game.EndTurn(game.Opponent));
        }
        #endregion

        #region DrawCards
        [Fact]
        public void DrawCards_Effect_Draws()
        {
            var game = CreateTestGame();
            game.Start();

            var handSize = game.CurrentPlayer.Hand.Count;
            game.DrawCards(game.CurrentPlayer, 1, DrawEventSource.Effect);
            Assert.Equal(handSize + 1, game.CurrentPlayer.Hand.Count);
        }

        [Fact]
        public void DrawCards_Multiple_Draws()
        {
            var game = CreateTestGame();
            game.Start();

            var handSize = game.CurrentPlayer.Hand.Count;
            game.DrawCards(game.CurrentPlayer, 2, DrawEventSource.Effect);
            Assert.Equal(handSize + 2, game.CurrentPlayer.Hand.Count);
        }

        [Fact]
        public void DrawCards_HandFull_DestroysExtras()
        {
            var game = CreateTestGame();
            game.Start();

            var handRoom = game.Options.MaximumHandSize - game.CurrentPlayer.Hand.Count;
            game.DrawCards(game.CurrentPlayer, handRoom + 1, DrawEventSource.Effect);
            Assert.Equal(game.Options.MaximumHandSize, game.CurrentPlayer.Hand.Count);
            Assert.Single(game.CurrentPlayer.Graveyard);
        }

        [Fact]
        public void DrawCards_DeckEmpty_DamagePlayer()
        {
            var game = CreateTestGame(6);
            game.Start();

            game.DrawCards(game.CurrentPlayer, game.CurrentPlayer.Deck.Count + 1, DrawEventSource.Effect);
            Assert.Equal(game.CurrentPlayer.InitialHealth - 1, game.CurrentPlayer.CurrentHealth);
        }
        #endregion

        #region AttackCreature
        [Fact]
        public void AttackCreature_SoldierSoldier_BothDestroyed()
        {
            var game = CreateTestGame();
            game.Start();

            // 1) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 1) Attack the opponent's basic soldier
            game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], game.Opponent.Field[0]);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);
        }

        [Fact]
        public void AttackCreature_SoldierGunner_SoldierDestroyedGunnerDamaged()
        {
            var game = CreateTestGame();
            game.Start();

            game.Opponent.Hand = new List<Card> { new Gunner() };
            
            // 1) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 1) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 gunner and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 1) Attack the gunner with the soldier
            var gunner = game.Opponent.Field[0];
            game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], gunner);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Single(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Empty(game.Opponent.Graveyard);

            Assert.Equal(gunner.Base.Health - 1, gunner.Health);
        }

        [Fact]
        public void AttackCreature_GunnerSoldier_GunnerDamagedSoldierDestroyed()
        {
            var game = CreateTestGame();
            game.Start();

            game.CurrentPlayer.Hand = new List<Card> { new Gunner() };

            // 1) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 1) Play 1 gunner and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 1) Attack the soldier with the gunner
            var gunner = game.CurrentPlayer.Field[0];
            game.AttackCreature(game.CurrentPlayer, gunner, game.Opponent.Field[0]);

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
            var game = CreateTestGame();
            game.Start();

            // 1) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 basic soldier and try to attack
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], game.Opponent.Field[0]));
        }

        [Fact]
        public void AttackCreature_Taunt_AttackOther_Throws()
        {
            var game = CreateTestGame();
            game.Start();

            // 1) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 1) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 2) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 1) Play a defender
            game.CurrentPlayer.Hand = new List<Card> { new Defender() };
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) Try to attack the soldier
            var soldier = game.Opponent.Field.First(c => c.Base is BasicSoldier);
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], soldier));
        }

        [Fact]
        public void AttackCreature_Taunt_AttackTaunter_Ok()
        {
            var game = CreateTestGame();
            game.Start();

            // 1) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 1) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 2) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 1) Play a defender
            game.CurrentPlayer.Hand = new List<Card> { new Defender() };
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) Attack the defender
            var defender = game.Opponent.Field.First(c => c.Base is Defender);
            game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], defender);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Equal(2, game.Opponent.Field.Count);

            Assert.Equal(defender.Base.Health - 1, defender.Health);
        }

        [Fact]
        public void AttackCreature_OwnField_Throws()
        {
            var game = CreateTestGame();
            game.Start();

            // 1) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 1) Play a gunner and try to attack the gunner with the soldier
            game.CurrentPlayer.Hand = new List<Card> { new Gunner() };
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            var soldier = game.CurrentPlayer.Field.First(c => c.Base is BasicSoldier);
            var gunner = game.CurrentPlayer.Field.First(c => c.Base is Gunner);
            Assert.Throws<Exception>(() =>
                game.AttackCreature(game.CurrentPlayer, soldier, gunner));
        }

        [Fact]
        public void AttackCreature_Rush_AttackImmediately()
        {
            var game = CreateTestGame();
            game.Start();

            // 1) End turn
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 basic soldier and end the turn
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 1) Play 1 quickshot and attack immediately
            game.CurrentPlayer.Hand = new List<Card> { new Quickshot() };
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.AttackCreature(game.CurrentPlayer, game.CurrentPlayer.Field[0], game.Opponent.Field[0]);

            Assert.Empty(game.CurrentPlayer.Field);
            Assert.Empty(game.Opponent.Field);
            Assert.Single(game.CurrentPlayer.Graveyard);
            Assert.Single(game.Opponent.Graveyard);
        }

        [Fact]
        public void AttackCreature_AttackerEffect_Proc()
        {
            var game = CreateTestGame();
            game.Start();

            // 1) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 2) End the turn
            game.EndTurn(game.CurrentPlayer);

            // 1) Play 1 booster
            game.CurrentPlayer.Hand = new List<Card> { new Attacker() };
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 2) Play 1 gunner and end the turn
            game.CurrentPlayer.Hand = new List<Card> { new Gunner() };
            game.PlayCreatureFromHand(game.CurrentPlayer, game.CurrentPlayer.Hand[0] as CreatureCard);
            game.EndTurn(game.CurrentPlayer);

            // 1) Attack the gunner with the attacker
            var attacker = game.CurrentPlayer.Field.First(c => c.Base is Attacker);
            var gunner = game.Opponent.Field.First(c => c.Base is Gunner);
            game.AttackCreature(game.CurrentPlayer, attacker, gunner);

            Assert.Equal(1, gunner.Health);
            Assert.Equal(1, attacker.Health);
        }
        #endregion

        #region Helpers
        // Creates a test game with 2 players with test decks of 10 basic soldiers
        private GameInstance CreateTestGame(int deckSize = 20)
        {
            var options = new GameInstanceOptions();
            var playerOne = CreateTestPlayer("Player 1", deckSize);
            var playerTwo = CreateTestPlayer("Player 2", deckSize);

            return _factoryFixture.GameFactory.Create(options, playerOne, playerTwo);
        }

        // Creates a test player with a deck made only of basic soldiers
        // Use this to test basic mechanics of the game
        private Player CreateTestPlayer(string name, int deckSize)
            => new()
            {
                Name = name,
                Deck = new List<(Card, int)>
                {
                    (new BasicSoldier(), deckSize)
                }
            };

        // Creates a sample game with 2 players with sample decks
        private GameInstance CreateSampleGame()
        {
            var options = new GameInstanceOptions();
            var playerOne = CreateSamplePlayer("Player 1");
            var playerTwo = CreateSamplePlayer("Player 2");

            return _factoryFixture.GameFactory.Create(options, playerOne, playerTwo);
        }

        // Creates a sample player with a working deck
        private Player CreateSamplePlayer(string name)
            => new()
            {
                Name = name,
                Deck = new List<(Card, int)>
                {
                    (new BasicSoldier(), 4),
                    (new Gunner(), 3),
                    (new Defender(), 3)
                }
            };
        #endregion
    }
}
