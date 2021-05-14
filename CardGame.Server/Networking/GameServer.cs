﻿using CardGame.Server.Handlers;
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
        private readonly ClientMessageHandler clientMessageHandler;
        private EventBasedNetListener listener;
        private NetManager server;
        private BackgroundWorker worker;
        private readonly Random rand = new Random();

        private readonly Dictionary<NetPeer, Guid> peerIds = new Dictionary<NetPeer, Guid>();

        public string Key { get; private set; }
        public List<NetPeer> ConnectedClients => server.ConnectedPeerList;
        
        public event EventHandler<Guid> ClientConnected;
        public event EventHandler<ClientMessageWrapper> MessageReceived;

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
            listener = new EventBasedNetListener();
            server = new NetManager(listener);
            server.Start("127.0.0.1", "::1", port);

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
