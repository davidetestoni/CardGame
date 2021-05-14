using CardGame.Server.Handlers;
using CardGame.Shared.Messages;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace CardGame.Server.Networking
{
    public class GameServer
    {
        private ClientMessageHandler clientMessageHandler;
        private EventBasedNetListener listener;
        private NetManager server;
        private BackgroundWorker worker;
        private readonly Random rand = new();

        public string Key { get; private set; }
        public List<NetPeer> ConnectedClients => server.ConnectedPeerList;
        
        public event EventHandler<NetPeer> ClientConnected;
        public event EventHandler<string> MessageReceived;

        public GameServer(ClientMessageHandler clientMessageHandler)
        {
            this.clientMessageHandler = clientMessageHandler;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!worker.CancellationPending)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
        }

        public GameServer Start(int port)
        {
            listener = new();
            server = new(listener);
            server.Start(port);

            Key = RandomString(8);

            // Handle connection requests
            listener.ConnectionRequestEvent += request =>
            {
                // Allow maximum 2 players
                if (server.ConnectedPeersCount < 2)
                {
                    request.AcceptIfKey(Key);
                }
                else
                {
                    request.Reject();
                }
            };

            listener.PeerConnectedEvent += peer =>
            {
                ClientConnected?.Invoke(this, peer);
                
                // Send hello to the peer in a reliable way
                NetDataWriter writer = new();
                writer.Put("HELLO");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            };

            listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
            {
                // Get a string with maximum 10000 characters
                var message = reader.GetString(10000);
                clientMessageHandler.Handle(message);
                reader.Recycle();
            };

            worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            worker.DoWork += Worker_DoWork;

            return this;
        }

        public void Close()
        {
            worker.CancelAsync();
            server.Stop();
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
