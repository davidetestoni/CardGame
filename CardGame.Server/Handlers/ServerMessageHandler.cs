using CardGame.Server.Networking;
using CardGame.Shared.Messages.Server;
using LiteNetLib;
using System;

namespace CardGame.Server.Handlers
{
    public class ServerMessageHandler
    {
        private readonly GameServer server;

        public ServerMessageHandler(GameServer server)
        {
            this.server = server;
        }

        public void BroadcastMessage(ServerMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            var serialized = ServerMessageSerializer.Serialize(message);
            server.BroadcastMessage(serialized, deliveryMethod);
        }

        public void SendMessage(ServerMessage message, Guid playerId, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            var serialized = ServerMessageSerializer.Serialize(message);
            server.SendMessage(serialized, playerId, deliveryMethod);
        }
    }
}
