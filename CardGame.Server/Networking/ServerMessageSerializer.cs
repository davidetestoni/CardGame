using CardGame.Shared.Messages.Server;
using Newtonsoft.Json;

namespace CardGame.Server.Networking
{
    internal static class ServerMessageSerializer
    {
        private static readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        internal static string Serialize(ServerMessage message)
            => JsonConvert.SerializeObject(message, jsonSettings);
    }
}
