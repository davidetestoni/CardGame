using CardGame.Server.Handlers;
using CardGame.Server.Instance.Logging;
using CardGame.Server.Networking;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Linq;
using System.Threading;

namespace CardGame.Server.Instance
{
    class Program
    {
        static readonly Random rand = new();

        static void Main(string[] args)
        {
            var port = 9050;

            var clientMessageHandler = new ClientMessageHandler();
            var server = new GameServer(clientMessageHandler).Start(port);
            Log.Info($"Server started on port {port}");

            server.ClientConnected += (sender, client) =>
            {
                Log.Info($"Client connected: {client}");
            };

            Log.Info($"Key: {server.Key}");

            Console.ReadLine();
        }

        private static void NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            // Get a string with maximum 10000 characters
            var message = reader.GetString(10000);
            Log.ClientMessage(message, peer);
            reader.Recycle();

            HandleMessage(message);
        }

        static void HandleMessage(string message)
        {
            try
            {

            }
            catch
            {
                Log.Error($"Invalid message: {message}");
            }
        }
    }
}
