using LiteNetLib;
using System;
using System.ComponentModel;
using System.Threading;

namespace CardGame.Client.Networking
{
    public class GameClient
    {
        private EventBasedNetListener listener;
        private NetManager client;
        private BackgroundWorker worker;

        public event EventHandler<string> MessageReceived;

        public GameClient Connect(string host, int port, string key)
        {
            listener = new();

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                var message = dataReader.GetString(10000);
                MessageReceived?.Invoke(this, message);
                dataReader.Recycle();
            };

            client = new(listener);
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
