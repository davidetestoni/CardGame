using CardGame.Client.Handlers;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace CardGame.Client.Networking
{
    public class GameClient
    {
        private EventBasedNetListener listener;
        private NetManager client;
        private BackgroundWorker worker;
        private readonly ServerMessageHandler serverMessageHandler;

        public event EventHandler<string> MessageReceived;
        public event EventHandler<string> MessageSent;
        public event EventHandler Connected;

        public GameClient(ServerMessageHandler serverMessageHandler)
        {
            this.serverMessageHandler = serverMessageHandler;
        }

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

        public void SendMessage(string message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            var server = client.ConnectedPeerList.First();
            server.Send(writer, deliveryMethod);
            MessageSent?.Invoke(this, message);
        }

        public void Close()
        {
            worker.CancelAsync();
            client.Stop();
        }

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
