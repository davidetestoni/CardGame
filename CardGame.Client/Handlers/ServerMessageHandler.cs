using CardGame.Shared.Messages.Server;
using Newtonsoft.Json;
using System;

namespace CardGame.Client.Handlers
{
    /// <summary>
    /// Handles deserializing messages from the server.
    /// </summary>
    public class ServerMessageHandler
    {
        private readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };

        /// <summary>
        /// An message has been received from the server but the deserialization failed.
        /// </summary>
        public event EventHandler<string> InvalidMessageReceived;

        /// <summary>
        /// A valid message has been received from the server and deserialized.
        /// </summary>
        public event EventHandler<ServerMessage> MessageReceived;

        /// <summary>
        /// An exception other than a <see cref="JsonException"/> occurred during the deserialization.
        /// </summary>
        public event EventHandler<Exception> Exception;

        internal void Handle(string str)
        {
            ServerMessage message;

            try
            {
                message = JsonConvert.DeserializeObject<ServerMessage>(str, jsonSettings);
                MessageReceived?.Invoke(this, message);
            }
            catch (JsonException)
            {
                InvalidMessageReceived?.Invoke(this, str);
            }
            catch (Exception ex)
            {
                Exception?.Invoke(this, ex);
            }
        }
    }
}
