using CardGame.Shared.Messages.Client;
using Newtonsoft.Json;

namespace CardGame.Server.Networking
{
    internal static class ClientMessageSerializer
    {
        private static readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        internal static string Serialize(ClientMessage message)
            => JsonConvert.SerializeObject(message, jsonSettings);
    }
}
