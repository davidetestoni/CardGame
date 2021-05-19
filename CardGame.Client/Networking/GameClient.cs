using CardGame.Client.Handlers;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace CardGame.Client.Networking
{
    /// <summary>
    /// The client that talks with the game server.
    /// </summary>
    public class GameClient
    {
        private EventBasedNetListener listener;
        private NetManager client;
        private BackgroundWorker worker;
        private readonly ServerMessageHandler serverMessageHandler;

        /// <summary>
        /// Called when a raw message is received from the server.
        /// </summary>
        public event EventHandler<string> MessageReceived;

        /// <summary>
        /// Called when a raw message is sent to the server.
        /// </summary>
        public event EventHandler<string> MessageSent;

        /// <summary>
        /// Called when the client connected to the game server.
        /// </summary>
        public event EventHandler Connected;

        public GameClient(ServerMessageHandler serverMessageHandler)
        {
            this.serverMessageHandler = serverMessageHandler;
        }

        /// <summary>
        /// Connects the client to a server with the given <paramref name="host"/> and
        /// <paramref name="port"/>, which requires a <paramref name="key"/> to grant access.
        /// </summary>
        public GameClient Connect(string host, int port, string key)
        {
            listener = new EventBasedNetListener();

            listener.PeerConnectedEvent += peer =>
            {
                Connected?.Invoke(this, EventArgs.Empty);
            };

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                var message = dataReader.GetString(10000);
                MessageReceived?.Invoke(this, message);
                serverMessageHandler.Handle(message);
                dataReader.Recycle();
            };

            client = new NetManager(listener);
            client.Start();
            client.Connect(host, port, key);

            worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();

            return this;
        }

        /// <summary>
        /// Sends a raw <paramref name="message"/> to the server with the given <paramref name="deliveryMethod"/>.
        /// </summary>
        public void SendMessage(string message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            var server = client.ConnectedPeerList.First();
            server.Send(writer, deliveryMethod);
            MessageSent?.Invoke(this, message);
        }

        /// <summary>
        /// Closes the connection to the server.
        /// </summary>
        public void Close()
        {
            worker.CancelAsync();
            client.Stop();
        }

        // Periodically poll new incoming messages from the server.
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!worker.CancellationPending)
            {
                client.PollEvents();
                Thread.Sleep(15);
            }
        }
    }
}
