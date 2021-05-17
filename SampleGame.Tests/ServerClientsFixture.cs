using CardGame.Server.Instances.Game;
using CardGame.Server.Networking;

namespace SampleGame.Tests
{
    public class ServerClientsFixture
    {
        public GameServer Server { get; set; }
        public GameInstance Game { get; set; }

        public ServerClientsFixture()
        {

        }
    }
}
