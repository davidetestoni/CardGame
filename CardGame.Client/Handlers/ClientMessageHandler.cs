﻿using CardGame.Client.Networking;
using CardGame.Server.Networking;
using CardGame.Shared.Messages.Client;
using LiteNetLib;

namespace CardGame.Client.Handlers
{
    public class ClientMessageHandler
    {
        private readonly GameClient client;

        public ClientMessageHandler(GameClient client)
        {
            this.client = client;
        }

        public void SendMessage(ClientMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            var serialized = ClientMessageSerializer.Serialize(message);
            client.SendMessage(serialized, deliveryMethod);
        }
    }
}
