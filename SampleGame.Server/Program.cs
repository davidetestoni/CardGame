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
            var clientMessageHandler = new ClientMessageHandler(); // Takes care of receiving messages from clients and deserializing them
            var playerActionHandler = new PlayerActionHandler(game, clientMessageHandler); // Performs the actions sent by clients on the game instance
            var server = new GameServer(clientMessageHandler).Start(port);
            var gameEventHandler = new GameEventHandler(game, server); // Takes care of sending game events to clients

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
