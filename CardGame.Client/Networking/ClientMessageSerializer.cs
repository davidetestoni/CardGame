using CardGame.Shared.Messages.Client;
using Newtonsoft.Json;

namespace CardGame.Server.Networking
{
    public static class ClientMessageSerializer
    {
        private static readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        public static string Serialize(ClientMessage message)
            => JsonConvert.SerializeObject(message, jsonSettings);
    }
}
