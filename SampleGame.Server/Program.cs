using CardGame.Server.Events.Game;
using CardGame.Server.Factories;
using CardGame.Server.Handlers;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Networking;
using CardGame.Shared.Models.Cards;
using CardGame.Shared.Models.Players;
using SampleGame.Cards.Creatures;
using SampleGame.Server.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace SampleGame.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 9050;

            var cardFactory = new CardInstanceFactory(typeof(BasicSoldier).Assembly);
            var playerFactory = new PlayerInstanceFactory();
            var gameFactory = new GameInstanceFactory(cardFactory, playerFactory);

            var gameOptions = new GameInstanceOptions();

            // These are just to test
            var player1 = new Player { Name = "Player One", Deck = new List<(Card, int)> { (new BasicSoldier(), 10) } };
            var player2 = new Player { Name = "Player Two", Deck = new List<(Card, int)> { (new BasicSoldier(), 10) } };

            var game = gameFactory.Create(gameOptions, player1, player2);

            var clientMessageHandler = new ClientMessageHandler();
            var server = new GameServer(clientMessageHandler).Start(port);
            var gameEventHandler = new GameEventHandler(game, server);
            var playerActionHandler = new PlayerActionHandler(game, clientMessageHandler, gameEventHandler);

            Log.Info($"Server started on port {port}");

            server.ClientConnected += (sender, client) =>
            {
                Log.Info($"Client {client} connected");
            };

            server.MessageReceived += (sender, message) =>
            {
                Log.ClientMessage(message.Body, message.SenderId);
            };

            clientMessageHandler.InvalidMessageReceived += (sender, message) =>
            {
                Log.Error($"Invalid message: {message.Body}");
            };

            game.GameStarted += (sender, e) => Log.FormattedGameEvent($"Game started. [darkorange]{e.CurrentPlayer.Name}[/] goes first");
            game.GameEnded += (sender, e) => Log.FormattedGameEvent($"Game ended. The winner is [darkorange]{e.Winner.Name}[/]");
            game.NewTurn += (sender, e) => 
            { 
                if (e.TurnNumber != 1)
                {
                    Log.FormattedGameEvent($"Turn ended. [darkorange]{e.CurrentPlayer.Name}[/]'s turn began");
                }
                
                LogBoard(game);
            };

            game.CardsDrawn += (sender, e) => LogCardsDrawn(e);

            game.PlayerAttacked += (sender, e) => Log.FormattedGameEvent($"[red]{e.Attacker.ShortName}[/] attacked [darkorange]{e.Player.Name}[/] and dealt [greenyellow]{e.Damage}[/] damage");
            game.PlayerDamaged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] received [greenyellow]{e.Damage}[/] damage");
            game.PlayerHealthRestored += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s health restored by [greenyellow]{e.Amount}[/] (health: [greenyellow]{e.Player.CurrentHealth}[/])");
            game.PlayerManaRestored += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s mana restored by [greenyellow]{e.Amount}[/] (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");
            game.PlayerManaSpent += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] spent [greenyellow]{e.Amount}[/] mana (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");
            game.PlayerMaxManaIncreased += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s maximum mana increased by [greenyellow]{e.Increment}[/] (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");

            game.CreatureAttacked += (sender, e) => Log.FormattedGameEvent($"[red]{e.Attacker.ShortName}[/] attacked [darkorange]{e.Defender.ShortName}[/], dealt [greenyellow]{e.Damage}[/] damage and received [greenyellow]{e.RecoilDamage}[/] recoil damage");
            game.CreatureAttackChanged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/]'s attack changed to [greenyellow]{e.NewValue}[/]");
            game.CreatureDamaged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Target.ShortName}[/] took [greenyellow]{e.Damage}[/] damage");
            game.CreatureDestroyed += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Target.ShortName}[/] was destroyed");
            game.CreatureHealthIncreased += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/]'s health was increased by [greenyellow]{e.Amount}[/]");
            game.CreaturePlayed += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] played [darkorange]{e.Creature.ShortName}[/] from their hand");
            game.CreatureSpawned += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/] spawned on [darkorange]{e.Creature.Owner.Name}[/]'s field");

            Log.Info($"Key: {server.Key}");

            game.Start();

            Console.ReadLine();
        }

        private static void LogCardsDrawn(CardsDrawnEvent e)
        {
            Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] drew [greenyellow]{e.NewCards.Count}[/] card(s)");

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"Cards drawn");

            var table = new Table()
                .AddColumn("[darkorange]ShortName[/]")
                .AddColumn("[greenyellow]Id[/]");

            e.NewCards.ForEach(c => table.AddRow(c.ShortName, c.Id.ToString()));
            AnsiConsole.Render(table);
            AnsiConsole.WriteLine();
        }

        private static void LogBoard(GameInstance game)
        {
            AnsiConsole.WriteLine();
            LogHand(game.PlayerOne);
            AnsiConsole.WriteLine();
            LogField(game.PlayerOne);
            AnsiConsole.WriteLine();
            LogHand(game.PlayerTwo);
            AnsiConsole.WriteLine();
            LogField(game.PlayerTwo);
            AnsiConsole.WriteLine();
        }

        private static void LogField(PlayerInstance player)
        {
            AnsiConsole.MarkupLine($"[darkorange]{player.Name}[/]'s field");

            var table = new Table()
                .AddColumn("[darkorange]ShortName[/]")
                .AddColumn("[greenyellow]Id[/]")
                .AddColumn("[red]Health[/]")
                .AddColumn("[dodgerblue1]Attack[/]")
                .AddColumn("[plum3]Can attack[/]");

            player.Field.ForEach(c => table.AddRow(
                c.ShortName, 
                c.Id.ToString(),
                c.Health.ToString(),
                c.Attack.ToString(),
                (c.AttacksLeft > 0).ToString()));

            AnsiConsole.Render(table);
        }

        private static void LogHand(PlayerInstance player)
        {
            AnsiConsole.MarkupLine($"[darkorange]{player.Name}[/]'s hand");

            var table = new Table()
                .AddColumn("[darkorange]ShortName[/]")
                .AddColumn("[greenyellow]Id[/]");

            player.Hand.ForEach(c => table.AddRow(
                c.ShortName, 
                c.Id.ToString()));

            AnsiConsole.Render(table);
        }
    }
}
