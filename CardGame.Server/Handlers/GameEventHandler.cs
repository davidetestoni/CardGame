using CardGame.Server.Instances.Game;
using CardGame.Server.Networking;
using CardGame.Shared.Messages.Server;
using CardGame.Shared.Messages.Server.Cards.Creatures;
using CardGame.Shared.Messages.Server.Game;
using CardGame.Shared.Messages.Server.Players;
using LiteNetLib;
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
        private readonly GameServer server;

        public GameEventHandler(GameInstance game, GameServer server)
        {
            this.game = game;
            this.server = server;

            #region Game Events
            game.GameStarted += (sender, e) =>
            {
                var message = new GameStartedMessage
                {
                    CurrentPlayerId = e.CurrentPlayer.Id
                };

                BroadcastMessage(message);
            };

            game.GameEnded += (sender, e) =>
            {
                var message = new GameEndedMessage
                {
                    WinnerId = e.Winner.Id
                };

                BroadcastMessage(message);
            };

            game.NewTurn += (sender, e) =>
            {
                var message = new NewTurnMessage
                {
                    CurrentPlayerId = e.CurrentPlayer.Id,
                    TurnNumber = e.TurnNumber
                };

                BroadcastMessage(message);
            };

            game.CardsDrawn += (sender, e) =>
            {
                var message = new CardsDrawnMessage
                {
                    PlayerId = e.Player.Id,
                    NewCards = e.NewCards.Select(c => new DrawnCardDTO { Id = c.Id, ShortName = c.ShortName }).ToList()
                };

                BroadcastMessage(message);
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

                BroadcastMessage(message);
            };

            game.PlayerDamaged += (sender, e) =>
            {
                var message = new PlayerDamagedMessage
                {
                    PlayerId = e.Player.Id,
                    Damage = e.Damage
                };

                BroadcastMessage(message);
            };

            game.PlayerHealthRestored += (sender, e) =>
            {
                var message = new PlayerHealthRestoredMessage
                {
                    PlayerId = e.Player.Id,
                    Amount = e.Amount
                };

                BroadcastMessage(message);
            };

            game.PlayerManaSpent += (sender, e) =>
            {
                var message = new PlayerManaSpentMessage
                {
                    PlayerId = e.Player.Id,
                    Amount = e.Amount
                };

                BroadcastMessage(message);
            };

            game.PlayerManaRestored += (sender, e) =>
            {
                var message = new PlayerManaRestoredMessage
                {
                    PlayerId = e.Player.Id,
                    Amount = e.Amount
                };

                BroadcastMessage(message);
            };

            game.PlayerMaxManaIncreased += (sender, e) =>
            {
                var message = new PlayerMaxManaIncreasedMessage
                {
                    PlayerId = e.Player.Id,
                    Increment = e.Increment
                };

                BroadcastMessage(message);
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

                BroadcastMessage(message);
            };

            game.CreatureDamaged += (sender, e) =>
            {
                var message = new CreatureDamagedMessage
                {
                    TargetId = e.Target.Id,
                    Damage = e.Damage
                };

                BroadcastMessage(message);
            };

            game.CreatureAttackChanged += (sender, e) =>
            {
                var message = new CreatureAttackChangedMessage
                {
                    CreatureId = e.Creature.Id,
                    NewValue = e.NewValue
                };

                BroadcastMessage(message);
            };

            game.CreatureHealthIncreased += (sender, e) =>
            {
                var message = new CreatureHealthIncreasedMessage
                {
                    CreatureId = e.Creature.Id,
                    Amount = e.Amount
                };

                BroadcastMessage(message);
            };

            game.CreatureAttacksLeftChanged += (sender, e) =>
            {
                var message = new CreatureAttacksLeftChangedMessage
                {
                    CreatureId = e.Creature.Id,
                    CanAttack = e.CanAttack
                };

                BroadcastMessage(message);
            };

            game.CreatureDestroyed += (sender, e) =>
            {
                var message = new CreatureDestroyedMessage
                {
                    CreatureId = e.Target.Id
                };

                BroadcastMessage(message);
            };

            game.CreatureSpawned += (sender, e) =>
            {
                var message = new CreatureSpawnedMessage
                {
                    CreatureId = e.Creature.Id,
                    ShortName = e.Creature.ShortName
                };

                BroadcastMessage(message);
            };

            game.CreaturePlayed += (sender, e) =>
            {
                var message = new CreaturePlayedMessage
                {
                    PlayerId = e.Player.Id,
                    CreatureId = e.Creature.Id
                };

                BroadcastMessage(message);
            };
            #endregion
        }

        public void BroadcastMessage(ServerMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            var serialized = ServerMessageSerializer.Serialize(message);
            server.BroadcastMessage(serialized, deliveryMethod);
        }
    }
}
