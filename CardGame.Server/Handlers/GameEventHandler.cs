using CardGame.Server.Instances.Game;
using CardGame.Server.Networking;
using CardGame.Shared.Messages.Server;
using CardGame.Shared.Messages.Server.Game;
using LiteNetLib;

namespace CardGame.Server.Handlers
{
    public class GameEventHandler
    {
        private readonly GameInstance game;
        private readonly GameServer server;

        public GameEventHandler(GameInstance game, GameServer server)
        {
            this.game = game;
            this.server = server;

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
        }

        public void BroadcastMessage(ServerMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            var serialized = ServerMessageSerializer.Serialize(message);
            server.BroadcastMessage(serialized, deliveryMethod);
        }
    }
}
