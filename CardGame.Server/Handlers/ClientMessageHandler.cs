using CardGame.Server.Networking;
using CardGame.Shared.Messages.Client;
using Newtonsoft.Json;
using System;

namespace CardGame.Server.Handlers
{
    /// <summary>
    /// Handles deserializing messages from the client.
    /// </summary>
    public class ClientMessageHandler
    {
        private readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        /// <summary>
        /// An message has been received from the client but the deserialization failed.
        /// </summary>
        public event EventHandler<ClientMessageWrapper> InvalidMessageReceived;

        /// <summary>
        /// A valid message has been received from the client and deserialized.
        /// </summary>
        public event EventHandler<ClientMessage> MessageReceived;

        /// <summary>
        /// An exception other than a <see cref="JsonException"/> occurred during the deserialization.
        /// </summary>
        public event EventHandler<Exception> Exception;

        internal void Handle(string str, Guid senderId)
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
