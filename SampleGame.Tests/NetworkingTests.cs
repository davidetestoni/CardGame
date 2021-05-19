using CardGame.Client.Networking;
using CardGame.Server.Networking;
using System.Threading.Tasks;
using Xunit;

namespace SampleGame.Tests
{
    // TODO: Add E2E tests to make sure both GameInstances are synchronized properly.
    public class NetworkingTests
    {
        [Fact]
        public async Task StartServer_ConnectClient_ReceiveHello()
        {
            var port = 9050;

            var clientMessageHandler = new CardGame.Server.Handlers.ClientMessageHandler();
            var server = new GameServer(clientMessageHandler).Start("127.0.0.1", port);

            var serverMessageHandler = new CardGame.Client.Handlers.ServerMessageHandler();
            var client = new GameClient(serverMessageHandler);
            string serverHello = string.Empty;
            client.MessageReceived += (sender, message) => serverHello = message;
            client.Connect("127.0.0.1", port, server.Key);

            await Task.Delay(1000);

            Assert.Equal("HELLO", serverHello);
        }
    }
}
