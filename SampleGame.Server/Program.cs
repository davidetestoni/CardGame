using CardGame.Server.Factories;
using CardGame.Server.Handlers;
using CardGame.Server.Instances.Game;
using CardGame.Server.Networking;
using SampleGame.Cards.Creatures;
using SampleGame.Server.Logging;
using System;

namespace SampleGame.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 9050;

            var game = new GameInstance(new CardInstanceFactory(typeof(BasicSoldier).Assembly));
            var clientMessageHandler = new ClientMessageHandler();
            var server = new GameServer(clientMessageHandler).Start(port);
            Log.Info($"Server started on port {port}");

            server.ClientConnected += (sender, client) =>
            {
                Log.Info($"Client {client} connected");
            };

            server.MessageReceived += (sender, message) =>
            {
                Log.ClientMessage(message.Body, message.SenderId);
            };

            Log.Info($"Key: {server.Key}");

            Console.ReadLine();
        }
    }
}
