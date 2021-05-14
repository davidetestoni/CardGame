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

        public GameClient Connect(string host, int port, string key)
        {
            listener = new();
            client = new(listener);
            client.Start();
            client.Connect(host, port, key);

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                Console.WriteLine("We got: {0}", dataReader.GetString(10000 /* max length of string */));
                dataReader.Recycle();
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
