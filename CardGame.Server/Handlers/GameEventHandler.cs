using CardGame.Server.Extensions;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Shared.DTOs;
using CardGame.Shared.Messages.Server;
using CardGame.Shared.Messages.Server.Cards.Creatures;
using CardGame.Shared.Messages.Server.Game;
using CardGame.Shared.Messages.Server.Players;
using System.Collections.Generic;
using System.Linq;

namespace CardGame.Server.Handlers
{
    /// <summary>
    /// The purpose of this class is to listen to game events and turn them into
    /// messages of type <see cref="ServerMessage"/> that are sent to the clients.
    /// </summary>
    public class GameEventHandler
    {
        private readonly GameInstance game;
        private readonly ServerMessageHandler serverMessageHandler;

        public GameEventHandler(GameInstance game, ServerMessageHandler serverMessageHandler)
        {
            this.game = game;
            this.serverMessageHandler = serverMessageHandler;

            #region Game Events
            game.GameStarted += (sender, e) =>
            {
                // Message for player 1
                var p1Message = new GameStartedMessage
                {
                    MyTurn = e.CurrentPlayer.Id == game.PlayerOne.Id,
                    GameInfo = new StartGameInfoDTO
                    {
                        InitialHealth = game.Options.InitialHealth,
                        OpponentInfo = new OpponentInfoDTO
                        {
                            Id = game.PlayerTwo.Id,
                            Name = game.PlayerTwo.Name,
                            DeckSize = game.PlayerTwo.Deck.Count
                        }
                    },
                    Deck = ConvertDeck(game.PlayerOne)
                };

                serverMessageHandler.SendMessage(p1Message, game.PlayerOne.Id);

                // Message for player 2
                var p2Message = new GameStartedMessage
                {
                    MyTurn = e.CurrentPlayer.Id == game.PlayerTwo.Id,
                    GameInfo = new StartGameInfoDTO
                    {
                        InitialHealth = game.Options.InitialHealth,
                        OpponentInfo = new OpponentInfoDTO
                        {
                            Id = game.PlayerOne.Id,
                            Name = game.PlayerOne.Name,
                            DeckSize = game.PlayerOne.Deck.Count
                        }
                    },
                    Deck = ConvertDeck(game.PlayerTwo)
                };

                serverMessageHandler.SendMessage(p2Message, game.PlayerTwo.Id);
            };

            game.GameEnded += (sender, e) =>
            {
                var message = new GameEndedMessage
                {
                    WinnerId = e.Winner.Id
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.NewTurn += (sender, e) =>
            {
                var message = new NewTurnMessage
                {
                    CurrentPlayerId = e.CurrentPlayer.Id,
                    TurnNumber = e.TurnNumber
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CardsDrawn += (sender, e) =>
            {
                var message = new CardsDrawnMessage
                {
                    NewCards = e.NewCards.Select(c => c.Id).ToList(),
                    DeckSize = e.Player.Deck.Count,
                    Destroyed = e.Destroyed.Select(c => c.Id).ToList()
                };

                serverMessageHandler.SendMessage(message, e.Player.Id);

                var opponentMessage = new CardsDrawnOpponentMessage
                {
                    Amount = e.NewCards.Count,
                    DeckSize = e.Player.Deck.Count,
                    Destroyed = e.Destroyed.Select(c => new CardInfoDTO { Id = c.Id, ShortName = c.ShortName }).ToList()
                };

                serverMessageHandler.SendMessage(opponentMessage, game.GetOpponent(e.Player).Id);
            };
            #endregion

            #region Player Events
            game.PlayerAttacked += (sender, e) =>
            {
                var message = new PlayerAttackedMessage
                {
                    PlayerId = e.Player.Id,
                    AttackerId = e.Attacker.Id,
                    Damage = e.Damage
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.PlayerDamaged += (sender, e) =>
            {
                var message = new PlayerDamagedMessage
                {
                    PlayerId = e.Player.Id,
                    Damage = e.Damage
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.PlayerHealthRestored += (sender, e) =>
            {
                var message = new PlayerHealthRestoredMessage
                {
                    PlayerId = e.Player.Id,
                    Amount = e.Amount
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.PlayerManaSpent += (sender, e) =>
            {
                var message = new PlayerManaSpentMessage
                {
                    PlayerId = e.Player.Id,
                    Amount = e.Amount
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.PlayerManaRestored += (sender, e) =>
            {
                var message = new PlayerManaRestoredMessage
                {
                    PlayerId = e.Player.Id,
                    Amount = e.Amount
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.PlayerMaxManaIncreased += (sender, e) =>
            {
                var message = new PlayerMaxManaIncreasedMessage
                {
                    PlayerId = e.Player.Id,
                    Increment = e.Increment
                };

                serverMessageHandler.BroadcastMessage(message);
            };
            #endregion

            #region Creature Events
            game.CreatureAttacked += (sender, e) =>
            {
                var message = new CreatureAttackedMessage
                {
                    AttackerId = e.Attacker.Id,
                    DefenderId = e.Defender.Id,
                    Damage = e.Damage,
                    RecoilDamage = e.RecoilDamage
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CreatureDamaged += (sender, e) =>
            {
                var message = new CreatureDamagedMessage
                {
                    TargetId = e.Target.Id,
                    Damage = e.Damage
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CreatureAttackChanged += (sender, e) =>
            {
                var message = new CreatureAttackChangedMessage
                {
                    CreatureId = e.Creature.Id,
                    NewValue = e.NewValue
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CreatureHealthIncreased += (sender, e) =>
            {
                var message = new CreatureHealthIncreasedMessage
                {
                    CreatureId = e.Creature.Id,
                    Amount = e.Amount
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CreatureAttacksLeftChanged += (sender, e) =>
            {
                var message = new CreatureAttacksLeftChangedMessage
                {
                    CreatureId = e.Creature.Id,
                    CanAttack = e.CanAttack
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CreatureDestroyed += (sender, e) =>
            {
                var message = new CreatureDestroyedMessage
                {
                    CreatureId = e.Target.Id
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CreatureSpawned += (sender, e) =>
            {
                var message = new CreatureSpawnedMessage
                {
                    PlayerId = e.Creature.Owner.Id,
                    CreatureId = e.Creature.Id,
                    ShortName = e.Creature.ShortName
                };

                serverMessageHandler.BroadcastMessage(message);
            };

            game.CreaturePlayed += (sender, e) =>
            {
                var message = new CreaturePlayedMessage
                {
                    CreatureId = e.Creature.Id
                };

                serverMessageHandler.SendMessage(message, e.Player.Id);

                var opponentMessage = new CreaturePlayedOpponentMessage
                {
                    CreatureId = e.Creature.Id,
                    ShortName = e.Creature.ShortName
                };

                serverMessageHandler.SendMessage(opponentMessage, game.GetOpponent(e.Player).Id);
            };
            #endregion
        }

        private List<CardInfoDTO> ConvertDeck(PlayerInstance player)
            => player.Deck.Select(c => new CardInfoDTO
            {
                Id = c.Id,
                ShortName = c.ShortName
            }).ToList().Shuffle(game.Random).ToList();
    }
}
