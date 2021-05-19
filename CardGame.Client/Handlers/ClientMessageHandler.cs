using CardGame.Client.Networking;
using CardGame.Server.Networking;
using CardGame.Shared.Messages.Client;
using LiteNetLib;

namespace CardGame.Client.Handlers
{
    /// <summary>
    /// Handles serializing and sending messages from the client to the server.
    /// </summary>
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
