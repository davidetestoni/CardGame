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

        public void Handle(string str)
        {
            // Try to deserialize the message
            ServerMessage message;

            try
            {
                message = JsonConvert.DeserializeObject<ServerMessage>(str, jsonSettings);
                MessageReceived?.Invoke(this, message);
            }
            catch
            {
                InvalidMessageReceived?.Invoke(this, str);
                return;
            }
        }
    }
}
