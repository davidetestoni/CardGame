using CardGame.Client.Handlers;
using CardGame.Client.Instances.Game;
using CardGame.Client.Networking;
using CardGame.Shared.Messages.Client.System;
using CardGame.Shared.Messages.Server.System;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace SampleGame.Client.Console
{
    class Program
    {
        private static GameInstanceOptions gameOptions;
        private static GameInstance game;
        private static GameClient client;
        private static ServerMessageHandler serverMessageHandler;
        private static ClientMessageHandler clientMessageHandler;
        private static Guid myId;

        static void Main(string[] args)
        {
            var host = "127.0.0.1";
            var port = 9050;

            var name = "Test";
            var deck = new Dictionary<string, int>
            {
                { "BasicSoldier", 10 }
            };

            serverMessageHandler = new ServerMessageHandler();
            client = new GameClient(serverMessageHandler);
            clientMessageHandler = new ClientMessageHandler(client);

            client.Connected += (sender, e) =>
            {
                clientMessageHandler.SendMessage(new JoinGameRequest
                {
                    Deck = deck,
                    PlayerName = name
                });
            };

            client.MessageReceived += (sender, message) =>
            {
                AnsiConsole.WriteLine(message);
            };

            serverMessageHandler.MessageReceived += (sender, message) =>
            {
                switch (message)
                {
                    case JoinGameResponse x:
                        myId = x.Id;
                        gameOptions = new GameInstanceOptions
                        {
                            InitialHealth = x.GameInfo.InitialHealth,
                            FieldSize = x.GameInfo.FieldSize
                        };
                        AnsiConsole.MarkupLine($"Game joined. My id is [darkorange]{myId}[/]");
                        break;
                }
            };

            var key = AnsiConsole.Ask<string>("Key");
            name = AnsiConsole.Ask<string>("Name");

            client.Connect(host, port, key);
            System.Console.ReadLine();
        }

        private static string MultipleChoice(string title, IList<string> choices)
            => AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                    .AddChoices(choices));

        private static int MultipleChoiceInt(string title, IList<string> choices)
            => choices.IndexOf(MultipleChoice(title, choices));
    }
}
