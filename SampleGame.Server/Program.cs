using CardGame.Server.Events.Game;
using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Networking;
using SampleGame.Cards.Creatures;
using SampleGame.Server.Logging;
using Spectre.Console;
using System;

namespace SampleGame.Server
{
    class Program
    {
        private static SingleGameServer server;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            var logReceivedMessages = false;
            var logSentMessages = false;

            var port = 9050;

            // TODO: Deserialize this from a json file
            var gameOptions = new GameInstanceOptions();
            var assembly = typeof(BasicSoldier).Assembly;

            server = new SingleGameServer("0.0.0.0", port, gameOptions, assembly);
            Log.Info($"Server started on port {port} with assembly {assembly.GetName().Name}");

            server.InnerServer.ClientConnected += (sender, client)
                => Log.Info($"Client {client} connected");

            if (logReceivedMessages)
            {
                server.InnerServer.MessageReceived += (sender, message)
                    => Log.ClientMessage(message.Body, message.SenderId);
            }

            if (logSentMessages)
            {
                server.InnerServer.MessageSent += (sender, message)
                    => Log.ServerMessage(message.Body, message.SenderId);
            }

            server.ClientMessageHandler.InvalidMessageReceived += (sender, message)
                => Log.Error($"Invalid message: {message.Body}");

            server.ClientMessageHandler.Exception += (sender, ex)
                => Log.Exception(ex);

            server.PlayerJoined += (sender, player)
                => Log.FormattedInfo($"Player {player.Id} ([darkorange]{player.Name}[/]) joined");

            BindGameEvents();

            Log.Info($"Key: {server.InnerServer.Key}");
            Log.Info("Waiting for players to join...");
            Console.ReadLine();
        }

        #region Logging
        private static void BindGameEvents()
        {
            server.Game.GameStarted += (sender, e) =>
            {
                Log.FormattedGameEvent($"Game started. [darkorange]{e.CurrentPlayer.Name}[/] goes first");

                AnsiConsole.WriteLine();
                LogDeck(server.Game.PlayerOne);
                AnsiConsole.WriteLine();
                LogDeck(server.Game.PlayerTwo);
                AnsiConsole.WriteLine();
            };
            server.Game.GameEnded += (sender, e) => Log.FormattedGameEvent($"Game ended. The winner is [darkorange]{e.Winner.Name}[/]");
            server.Game.NewTurn += (sender, e) =>
            {
                if (e.TurnNumber != 1)
                {
                    Log.FormattedGameEvent($"Turn ended. [darkorange]{e.CurrentPlayer.Name}[/]'s turn began");
                }

                LogBoard();
            };

            server.Game.CardsDrawn += (sender, e) => LogCardsDrawn(e);

            server.Game.PlayerAttacked += (sender, e) => Log.FormattedGameEvent($"[red]{e.Attacker.ShortName}[/] attacked [darkorange]{e.Player.Name}[/] and dealt [greenyellow]{e.Damage}[/] damage");
            server.Game.PlayerDamaged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] received [greenyellow]{e.Damage}[/] damage");
            server.Game.PlayerHealthRestored += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s health restored by [greenyellow]{e.Amount}[/] (health: [greenyellow]{e.Player.CurrentHealth}[/])");
            server.Game.PlayerManaRestored += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s mana restored by [greenyellow]{e.Amount}[/] (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");
            server.Game.PlayerManaSpent += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] spent [greenyellow]{e.Amount}[/] mana (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");
            server.Game.PlayerMaxManaIncreased += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s maximum mana increased by [greenyellow]{e.Increment}[/] (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");

            server.Game.CreatureAttacked += (sender, e) => Log.FormattedGameEvent($"[red]{e.Attacker.ShortName}[/] attacked [darkorange]{e.Defender.ShortName}[/], dealt [greenyellow]{e.Damage}[/] damage and received [greenyellow]{e.RecoilDamage}[/] recoil damage");
            server.Game.CreatureAttackChanged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/]'s attack changed to [greenyellow]{e.NewValue}[/]");
            server.Game.CreatureDamaged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Target.ShortName}[/] took [greenyellow]{e.Damage}[/] damage");
            server.Game.CreatureDestroyed += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Target.ShortName}[/] was destroyed");
            server.Game.CreatureHealthIncreased += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/]'s health was increased by [greenyellow]{e.Amount}[/]");
            server.Game.CreaturePlayed += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] played [darkorange]{e.Creature.ShortName}[/] from their hand");
            server.Game.CreatureSpawned += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/] spawned on [darkorange]{e.Creature.Owner.Name}[/]'s field");
        }

        private static void LogDeck(PlayerInstance player)
        {
            AnsiConsole.MarkupLine($"[darkorange]{player.Name}[/]'s deck");

            var table = new Table()
                .AddColumn("[darkorange]ShortName[/]")
                .AddColumn("[greenyellow]Id[/]");

            player.Deck.ForEach(c => table.AddRow(
                c.ShortName,
                c.Id.ToString()));

            AnsiConsole.Render(table);
        }

        private static void LogCardsDrawn(CardsDrawnEventArgs e)
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

        private static void LogBoard()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[darkorange]{server.Game.PlayerOne.Name}[/] ({server.Game.PlayerOne.Id})");
            LogPlayerInfo(server.Game.PlayerOne);
            LogHand(server.Game.PlayerOne);
            LogField(server.Game.PlayerOne);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[darkorange]{server.Game.PlayerTwo.Name}[/] ({server.Game.PlayerTwo.Id})");
            LogPlayerInfo(server.Game.PlayerTwo);
            LogHand(server.Game.PlayerTwo);
            LogField(server.Game.PlayerTwo);
        }

        private static void LogPlayerInfo(PlayerInstance player)
        {
            AnsiConsole.MarkupLine($"Health: [red]{player.CurrentHealth} / {player.InitialHealth}[/]");
            AnsiConsole.MarkupLine($"Mana: [dodgerblue1]{player.CurrentMana} / {player.MaximumMana}[/]");
            AnsiConsole.MarkupLine($"Deck: [greenyellow]{player.Deck.Count}[/]");
            AnsiConsole.MarkupLine($"Graveyard: [plum3]{player.Graveyard.Count}[/]");
        }

        private static void LogField(PlayerInstance player)
        {
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
            var table = new Table()
                .AddColumn("[darkorange]ShortName[/]")
                .AddColumn("[greenyellow]Id[/]");

            player.Hand.ForEach(c => table.AddRow(
                c.ShortName, 
                c.Id.ToString()));

            AnsiConsole.Render(table);
        }
        #endregion
    }
}
