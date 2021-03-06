using CardGame.Server.Handlers;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace CardGame.Server.Networking
{
    /// <summary>
    /// The game server that talks with the clients.
    /// </summary>
    public class GameServer
    {
        private readonly ClientMessageHandler clientMessageHandler;
        private EventBasedNetListener listener;
        private NetManager server;
        private BackgroundWorker worker;
        private readonly Random rand = new Random();

        private readonly Dictionary<NetPeer, Guid> peerIds = new Dictionary<NetPeer, Guid>();

        /// <summary>
        /// The key that clients need to send in order to be able to connect.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// The list of connected clients.
        /// </summary>
        public List<NetPeer> ConnectedClients => server.ConnectedPeerList;

        /// <summary>
        /// Called when a client connected to the game server.
        /// </summary>
        public event EventHandler<Guid> ClientConnected;

        /// <summary>
        /// Called when a raw message is received from the client.
        /// </summary>
        public event EventHandler<ClientMessageWrapper> MessageReceived;

        /// <summary>
        /// Called when a raw message is sent to the server.
        /// </summary>
        public event EventHandler<ClientMessageWrapper> MessageSent;

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

        /// <summary>
        /// Starts the game server on the given <paramref name="host"/> and
        /// <paramref name="port"/>. Generates a random access <see cref="Key"/>.
        /// </summary>
        public GameServer Start(string host, int port)
        {
            listener = new EventBasedNetListener();
            server = new NetManager(listener);
            server.Start(host, "::1", port);

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
                var id = Guid.NewGuid();
                peerIds[peer] = id;
                ClientConnected?.Invoke(this, id);
                
                // Send hello to the peer in a reliable way
                NetDataWriter writer = new NetDataWriter();
                writer.Put("HELLO");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            };

            listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
            {
                // Get a string with maximum 10000 characters
                var message = reader.GetString(10000);
                MessageReceived?.Invoke(this, new ClientMessageWrapper(message, peerIds[peer]));
                clientMessageHandler.Handle(message, peerIds[peer]);
                reader.Recycle();
            };

            worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();

            return this;
        }

        /// <summary>
        /// Sends a raw <paramref name="message"/> to both clients with the given <paramref name="deliveryMethod"/>.
        /// </summary>
        public void BroadcastMessage(string message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            foreach (var peerId in peerIds.Values)
            {
                SendMessage(message, peerId, deliveryMethod);
            }
        }

        /// <summary>
        /// Sends a raw <paramref name="message"/> to a client with the given <paramref name="deliveryMethod"/>.
        /// </summary>
        public void SendMessage(string message, Guid peerId, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            var peer = peerIds.First(p => p.Value == peerId).Key;
            peer.Send(writer, deliveryMethod);
            MessageSent?.Invoke(this, new ClientMessageWrapper(message, peerId));
        }

        /// <summary>
        /// Closes all connections and stops the server.
        /// </summary>
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
