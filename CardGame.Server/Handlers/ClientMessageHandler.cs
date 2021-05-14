using CardGame.Server.Networking;
using CardGame.Shared.Messages.Client;
using Newtonsoft.Json;
using System;

namespace CardGame.Server.Handlers
{
    public class ClientMessageHandler
    {
        private readonly JsonSerializerSettings jsonSettings =
            new() { TypeNameHandling = TypeNameHandling.Auto };

        public event EventHandler<ClientMessageWrapper> InvalidMessageReceived;
        public event EventHandler<ClientMessage> MessageReceived;

        public void Handle(string str, Guid senderId)
        {
            // Try to deserialize the message
            ClientMessage message;

            try
            {
                message = JsonConvert.DeserializeObject<ClientMessage>(str, jsonSettings);
                message.PlayerId = senderId;
                MessageReceived?.Invoke(this, message);
            }
            catch
            {
                InvalidMessageReceived?.Invoke(this, new(str, senderId));
                return;
            }
        }
    }
}
