using CardGame.Server.Networking;
using CardGame.Shared.Messages.Client;
using Newtonsoft.Json;
using System;

namespace CardGame.Server.Handlers
{
    public class ClientMessageHandler
    {
        private readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        public event EventHandler<ClientMessageWrapper> InvalidMessageReceived;
        public event EventHandler<ClientMessage> MessageReceived;
        public event EventHandler<Exception> Exception;

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
            catch (JsonException)
            {
                InvalidMessageReceived?.Invoke(this, new ClientMessageWrapper(str, senderId));
            }
            catch (Exception ex)
            {
                Exception?.Invoke(this, ex);
            }
        }
    }
}
