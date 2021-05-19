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
    /// <summary>
    /// A client that can join a single game of cards.
    /// </summary>
    public class SingleGameClient
    {
        /// <summary>
        /// The local replica of the game, synchronized with the one on the server.
        /// </summary>
        public GameInstance Game { get; private set; }

        /// <summary>
        /// Handles messages from the server.
        /// </summary>
        public ServerMessageHandler ServerMessageHandler { get; private set; }

        /// <summary>
        /// Handles messages from the client.
        /// </summary>
        public ClientMessageHandler ClientMessageHandler { get; private set; }

        /// <summary>
        /// Handles game events that happen because of a server message.
        /// </summary>
        public GameEventHandler GameEventHandler { get; private set; }

        /// <summary>
        /// Handles actions that can be performed by a player during the game.
        /// </summary>
        public PlayerActionHandler PlayerActionHandler { get; private set; }

        /// <summary>
        /// The underlying network client that manages the connection to the server.
        /// </summary>
        public GameClient InnerClient { get; private set; }

        /// <summary>
        /// The id of this client.
        /// </summary>
        public Guid MyId { get; private set; }

        private readonly CardInstanceFactory cardInstanceFactory;
        private readonly Player player;

        /// <summary>
        /// Called when a game is joined, and gives information about the id of your player in the game.
        /// </summary>
        public event EventHandler<Guid> GameJoined;

        /// <summary>
        /// Starts a client that connects to a server that hosts a single game which supports cards
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
