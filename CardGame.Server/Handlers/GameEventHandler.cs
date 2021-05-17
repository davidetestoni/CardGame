using CardGame.Server.Instances.Game;
using CardGame.Server.Networking;
using CardGame.Shared.Messages.Server;
using CardGame.Shared.Messages.Server.Cards.Creatures;
using CardGame.Shared.Messages.Server.Game;
using CardGame.Shared.Messages.Server.Players;
using LiteNetLib;
using System;
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
                var message = new GameStartedMessage
                {
                    CurrentPlayerId = e.CurrentPlayer.Id
                };

                serverMessageHandler.BroadcastMessage(message);
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
                    NewCards = e.NewCards.Select(c => new DrawnCardDTO { Id = c.Id, ShortName = c.ShortName }).ToList(),
                    DeckSize = e.Player.Deck.Count
                };

                serverMessageHandler.SendMessage(message, e.Player.Id);

                var opponentMessage = new CardsDrawnOpponentMessage
                {
                    Amount = e.NewCards.Count,
                    DeckSize = e.Player.Deck.Count
                };

                serverMessageHandler.SendMessage(message, game.GetOpponent(e.Player).Id);
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

                serverMessageHandler.SendMessage(message, game.GetOpponent(e.Player).Id);
            };
            #endregion
        }
    }
}
