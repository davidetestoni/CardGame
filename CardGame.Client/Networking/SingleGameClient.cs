using CardGame.Client.Factories;
using CardGame.Client.Handlers;
using CardGame.Client.Instances.Game;
using CardGame.Client.Instances.Players;
using CardGame.Shared.Messages.Client.System;
using CardGame.Shared.Messages.Server;
using CardGame.Shared.Messages.Server.System;
using CardGame.Shared.Models.Players;
using System;
using System.Reflection;

namespace CardGame.Client.Networking
{
    public class SingleGameClient
    {
        public GameInstance Game { get; private set; }
        public ServerMessageHandler ServerMessageHandler { get; private set; }
        public ClientMessageHandler ClientMessageHandler { get; private set; }
        public GameEventHandler GameEventHandler { get; private set; }
        public PlayerActionHandler PlayerActionHandler { get; private set; }
        public GameClient InnerClient { get; private set; }
        public Guid MyId { get; private set; }

        private readonly CardInstanceFactory cardInstanceFactory;
        private readonly Player player;

        public event EventHandler<Guid> GameJoined;

        /// <summary>
        /// Starts a client that connects to a server with the given <paramref name="host"/>
        /// and <paramref name="port"/> that hosts a single game which supports cards
        /// from the provided <paramref name="cardsAssembly"/>.
        /// </summary>
        public SingleGameClient(Assembly cardsAssembly, Player player)
        {
            ServerMessageHandler = new ServerMessageHandler();
            ServerMessageHandler.MessageReceived += HandleMessage;
            InnerClient = new GameClient(ServerMessageHandler);
            ClientMessageHandler = new ClientMessageHandler(InnerClient);
            PlayerActionHandler = new PlayerActionHandler(ClientMessageHandler);

            InnerClient.Connected += (sender, e) =>
            {
                ClientMessageHandler.SendMessage(new JoinGameRequest
                {
                    Deck = player.Deck,
                    PlayerName = player.Name
                });
            };

            cardInstanceFactory = new CardInstanceFactory(cardsAssembly);

            Game = new GameInstance(cardInstanceFactory);
            GameEventHandler = new GameEventHandler(Game, ServerMessageHandler);
            this.player = player;
        }

        private void HandleMessage(object sender, ServerMessage message)
        {
            switch (message)
            {
                case JoinGameResponse x:
                    MyId = x.PlayerId;
                    var gameOptions = new GameInstanceOptions
                    {
                        InitialHealth = x.GameInfo.InitialHealth,
                        FieldSize = x.GameInfo.FieldSize
                    };
                    Game.Me = new MeInstance
                    {
                        Name = player.Name,
                        Id = MyId,
                    };
                    GameJoined?.Invoke(this, MyId);
                    break;
            }
        }
    }
}
