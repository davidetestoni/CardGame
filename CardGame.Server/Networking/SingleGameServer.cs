using CardGame.Server.Factories;
using CardGame.Server.Handlers;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Shared.DTOs;
using CardGame.Shared.Messages.Client;
using CardGame.Shared.Messages.Client.System;
using CardGame.Shared.Messages.Server.System;
using CardGame.Shared.Models.Players;
using System;
using System.Reflection;
using System.Threading;

namespace CardGame.Server.Networking
{
    public class SingleGameServer
    {
        public GameInstance Game { get; private set; }
        public ClientMessageHandler ClientMessageHandler { get; private set; }
        public ServerMessageHandler ServerMessageHandler { get; private set; }
        public GameEventHandler GameEventHandler { get; private set; }
        public PlayerActionHandler PlayerActionHandler { get; private set; }
        public GameServer InnerServer { get; private set; }

        private readonly CardInstanceFactory cardInstanceFactory;
        private readonly GameInstanceFactory gameInstanceFactory;

        public event EventHandler<PlayerInstance> PlayerJoined;

        /// <summary>
        /// Starts a server on the given <paramref name="port"/> that can host a single game
        /// which supports cards from the provided <paramref name="cardsAssembly"/>.
        /// </summary>
        public SingleGameServer(string host, int port, GameInstanceOptions options, Assembly cardsAssembly)
        {
            ClientMessageHandler = new ClientMessageHandler();
            ClientMessageHandler.MessageReceived += HandleMessage;
            InnerServer = new GameServer(ClientMessageHandler).Start(host, port);
            ServerMessageHandler = new ServerMessageHandler(InnerServer);

            cardInstanceFactory = new CardInstanceFactory(cardsAssembly);
            gameInstanceFactory = new GameInstanceFactory(cardInstanceFactory);

            Game = gameInstanceFactory.Create(options);
            GameEventHandler = new GameEventHandler(Game, ServerMessageHandler);
            PlayerActionHandler = new PlayerActionHandler(Game, ClientMessageHandler, ServerMessageHandler);
        }

        private void HandleMessage(object sender, ClientMessage message)
        {
            switch (message)
            {
                case JoinGameRequest x:
                    if (Game.PlayerOne == null)
                    {
                        Game.PlayerOne = gameInstanceFactory.CreatePlayer(Game, new Player
                        {
                            Name = x.PlayerName,
                            Deck = x.Deck,
                            Id = x.PlayerId
                        });
                        ServerMessageHandler.SendMessage(new JoinGameResponse
                        {
                            PlayerId = x.PlayerId,
                            GameInfo = new GameInfoDTO
                            {
                                InitialHealth = Game.Options.InitialHealth,
                                FieldSize = Game.Options.FieldSize
                            }
                        }, x.PlayerId);
                        PlayerJoined?.Invoke(this, Game.PlayerOne);
                    }
                    else if (Game.PlayerTwo == null)
                    {
                        if (Game.PlayerOne.Id == x.PlayerId)
                        {
                            SendError("You already joined this game", x.PlayerId);
                        }
                        else
                        {
                            Game.PlayerTwo = gameInstanceFactory.CreatePlayer(Game, new Player
                            {
                                Name = x.PlayerName,
                                Deck = x.Deck,
                                Id = x.PlayerId
                            });
                            ServerMessageHandler.SendMessage(new JoinGameResponse
                            {
                                PlayerId = x.PlayerId,
                                GameInfo = new GameInfoDTO
                                {
                                    InitialHealth = Game.Options.InitialHealth,
                                    FieldSize = Game.Options.FieldSize
                                }
                            }, x.PlayerId);
                            PlayerJoined?.Invoke(this, Game.PlayerTwo);

                            // We have 2 players so we can start the game.
                            // HACK: Wait a bit so the clients have time to process
                            // the previous join response. Maybe in the future it's better to include the information
                            // directly into the previous message if the player connecting causes the game to start.
                            Thread.Sleep(1000);
                            Game.Start();
                        }
                    }
                    else
                    {
                        if (Game.PlayerOne.Id == x.PlayerId || Game.PlayerTwo.Id == x.PlayerId)
                        {
                            SendError("You already joined this game", x.PlayerId);
                        }
                        else
                        {
                            SendError("Game full, two players already joined", x.PlayerId);
                        }
                    }
                    break;
            }
        }

        private void SendError(string error, Guid playerId)
            => ServerMessageHandler.SendMessage(new ErrorResponse { Error = error }, playerId);
    }
}
