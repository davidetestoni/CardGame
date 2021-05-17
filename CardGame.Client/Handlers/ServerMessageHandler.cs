using CardGame.Shared.Messages.Server;
using Newtonsoft.Json;
using System;

namespace CardGame.Client.Handlers
{
    public class ServerMessageHandler
    {
        private readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };

        public event EventHandler<string> InvalidMessageReceived;
        public event EventHandler<ServerMessage> MessageReceived;
        public event EventHandler<Exception> Exception;

        public void Handle(string str)
        {
            // Try to deserialize the message
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
