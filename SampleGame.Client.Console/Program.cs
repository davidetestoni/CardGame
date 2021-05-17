using CardGame.Client.Events.Game;
using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;
using CardGame.Client.Models.PlayerActions.Cards.Creatures;
using CardGame.Client.Models.PlayerActions.Game;
using CardGame.Client.Networking;
using CardGame.Shared.Enums;
using CardGame.Shared.Models.Players;
using SampleGame.Cards.Creatures;
using SampleGame.Client.Console.Logging;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SampleGame.Client.Console
{
    class Program
    {
        private static SingleGameClient client;
        private static bool justEndedTurn;

        static void Main(string[] args)
        {
            System.Console.Title = "Client";
            var logReceivedMessages = false;
            var logSentMessages = false;

            var host = "127.0.0.1";
            var port = 9050;

            var player = new Player
            {
                Name = "Test",
                Deck = new Dictionary<string, int>
                {
                    { "BasicSoldier", 10 }
                }
            };

            client = new SingleGameClient(typeof(BasicSoldier).Assembly, player);
            BindGameEvents();

            client.GameJoined += (sender, playerId)
                => AnsiConsole.MarkupLine($"Game joined. My id is [darkorange]{playerId}[/]");

            if (logReceivedMessages)
            {
                client.InnerClient.MessageReceived += (sender, message)
                    => Log.ServerMessage(message);
            }

            if (logSentMessages)
            {
                client.InnerClient.MessageSent += (sender, message)
                    => Log.ClientMessage(message);
            }

            client.ServerMessageHandler.InvalidMessageReceived += (sender, message)
                => Log.Error($"Invalid message: {message}");

            client.ServerMessageHandler.Exception += (sender, ex)
                => Log.Exception(ex);

            client.GameEventHandler.Error += (sender, e) => Log.Error(e);

            var key = AnsiConsole.Ask<string>("Key");
            player.Name = AnsiConsole.Ask<string>("Name");
            System.Console.Title = $"Client - {player.Name}";

            client.InnerClient.Connect(host, port, key);

            Log.Info("Waiting for the game to start...");
            while (client.Game.Status == GameStatus.Created)
            {
                Thread.Sleep(1000);
            }

            while (client.Game.Status == GameStatus.Started)
            {
                if (client.Game.MyTurn && !justEndedTurn)
                {
                    ProcessPlayerAction();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            System.Console.ReadLine();
        }

        private static void BindGameEvents()
        {
            client.Game.GameStarted += (sender, e) => 
            {
                Log.FormattedGameEvent($"Game started. Your opponent is [darkorange]{client.Game.Opponent.Name}[/]. [darkorange]{e.CurrentPlayer.Name}[/] goes first");

                AnsiConsole.WriteLine();
                LogDeck();
                AnsiConsole.WriteLine();
            };
            client.Game.GameEnded += (sender, e) => Log.FormattedGameEvent($"Game ended, [darkorange]{e.Winner.Name}[/] is the winner");
            client.Game.NewTurn += (sender, e) =>
            {
                Log.FormattedGameEvent($"Turn ended. [darkorange]{e.CurrentPlayer.Name}[/] plays the next turn");
                LogBoard();
                justEndedTurn = false;
            };
            client.Game.CardsDrawn += (sender, e) => LogCardsDrawn(e);
            client.Game.CardsDrawnOpponent += (sender, e) => Log.FormattedGameEvent($"[darkorange]{client.Game.Opponent.Name}[/] drew [greenyellow]{e.Amount}[/] card(s)");

            client.Game.PlayerAttacked += (sender, e) => Log.FormattedGameEvent($"[red]{e.Attacker.ShortName}[/] attacked [darkorange]{e.Player.Name}[/] and dealt [greenyellow]{e.Damage}[/] damage");
            client.Game.PlayerDamaged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] received [greenyellow]{e.Damage}[/] damage");
            client.Game.PlayerHealthRestored += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s health restored by [greenyellow]{e.Amount}[/] (health: [greenyellow]{e.Player.CurrentHealth}[/])");
            client.Game.PlayerManaRestored += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s mana restored by [greenyellow]{e.Amount}[/] (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");
            client.Game.PlayerManaSpent += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] spent [greenyellow]{e.Amount}[/] mana (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");
            client.Game.PlayerMaxManaIncreased += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/]'s maximum mana increased by [greenyellow]{e.Increment}[/] (mana: [greenyellow]{e.Player.CurrentMana} / {e.Player.MaximumMana}[/])");

            client.Game.CreatureAttacked += (sender, e) => Log.FormattedGameEvent($"[red]{e.Attacker.ShortName}[/] attacked [darkorange]{e.Defender.ShortName}[/], dealt [greenyellow]{e.Damage}[/] damage and received [greenyellow]{e.RecoilDamage}[/] recoil damage");
            client.Game.CreatureAttackChanged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/]'s attack changed to [greenyellow]{e.NewValue}[/]");
            client.Game.CreatureDamaged += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Target.ShortName}[/] took [greenyellow]{e.Damage}[/] damage");
            client.Game.CreatureDestroyed += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Target.ShortName}[/] was destroyed");
            client.Game.CreatureHealthIncreased += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/]'s health was increased by [greenyellow]{e.Amount}[/]");
            client.Game.CreaturePlayed += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Player.Name}[/] played [darkorange]{e.Creature.ShortName}[/] from their hand");
            client.Game.CreatureSpawned += (sender, e) => Log.FormattedGameEvent($"[darkorange]{e.Creature.ShortName}[/] spawned on [darkorange]{e.Creature.Owner.Name}[/]'s field");
        }

        private static void LogBoard()
        {
            AnsiConsole.WriteLine();
            LogHand();
            AnsiConsole.WriteLine();
            LogField(client.Game.Me);
            AnsiConsole.WriteLine();
            LogField(client.Game.Opponent);
            AnsiConsole.WriteLine();
        }

        private static void ProcessPlayerAction()
        {
            CHOOSEACTION:
            var choice = MultipleChoiceInt("What do you want to do?", new List<string>
            {
                "Play card from hand",
                "Attack player",
                "Attack creature",
                "End turn",
                "Surrender"
            });

            switch (choice)
            {
                case 0:
                    var card = MultipleChoiceCard("Select the card", client.Game.Me.Hand);
                    if (card == null)
                    {
                        goto CHOOSEACTION;
                    }
                    client.PlayerActionHandler.PerformAction(new PlayCardAction
                    {
                        Card = card
                    });
                    break;

                case 1:
                    var attacker = MultipleChoiceCreature("Select the attacker", client.Game.Me.Field.Where(c => c.CanAttack));
                    if (attacker == null)
                    {
                        goto CHOOSEACTION;
                    }
                    client.PlayerActionHandler.PerformAction(new AttackPlayerAction
                    {
                        Attacker = attacker
                    });
                    break;

                case 2:
                    CHOOSECREATURE:
                    attacker = MultipleChoiceCreature("Select the attacker", client.Game.Me.Field.Where(c => c.CanAttack));
                    if (attacker == null)
                    {
                        goto CHOOSEACTION;
                    }
                    var target = MultipleChoiceCreature("Select the target", client.Game.Opponent.Field);
                    if (target == null)
                    {
                        goto CHOOSECREATURE;
                    }
                    client.PlayerActionHandler.PerformAction(new AttackCreatureAction
                    {
                        Attacker = attacker,
                        Target = target
                    });
                    break;

                case 3:
                    client.PlayerActionHandler.PerformAction(new EndTurnAction());
                    justEndedTurn = true;
                    break;

                case 4:
                    client.PlayerActionHandler.PerformAction(new SurrenderAction());
                    break;
            }
        }

        private static void LogDeck()
        {
            AnsiConsole.MarkupLine($"[darkorange]My[/] deck");

            var table = new Table()
                .AddColumn("[darkorange]ShortName[/]")
                .AddColumn("[greenyellow]Id[/]");

            client.Game.Me.Deck.ForEach(c => table.AddRow(
                c.ShortName,
                c.Id.ToString()));

            AnsiConsole.Render(table);
        }

        private static void LogCardsDrawn(CardsDrawnEvent e)
        {
            Log.FormattedGameEvent($"[darkorange]You[/] drew [greenyellow]{e.NewCards.Count}[/] card(s)");

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"Cards drawn");

            var table = new Table()
                .AddColumn("[darkorange]Name[/]");

            e.NewCards.ForEach(c => table.AddRow(c.Base.Name));
            AnsiConsole.Render(table);
            AnsiConsole.WriteLine();
        }

        private static void LogField(PlayerInstance player)
        {
            var name = player == client.Game.Me ? "[darkorange]Your[/]" : $"[darkorange]{player.Name}[/]'s";
            AnsiConsole.MarkupLine($"{name} field");

            var table = new Table()
                .AddColumn("[darkorange]Name[/]")
                .AddColumn("[red]Health[/]")
                .AddColumn("[dodgerblue1]Attack[/]")
                .AddColumn("[plum3]Can attack[/]");

            player.Field.ForEach(c => table.AddRow(
                c.Base.Name,
                c.Health.ToString(),
                c.Attack.ToString(),
                c.CanAttack.ToString()));

            AnsiConsole.Render(table);
        }

        private static void LogHand()
        {
            AnsiConsole.MarkupLine($"[darkorange]Your[/] hand");

            var table = new Table()
                .AddColumn("[darkorange]ShortName[/]")
                .AddColumn("[greenyellow]Id[/]");

            client.Game.Me.Hand.ForEach(c => table.AddRow(
                c.ShortName,
                c.Id.ToString()));

            AnsiConsole.Render(table);
        }

        private static string MultipleChoice(string title, IList<string> choices)
            => AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(choices));

        private static int MultipleChoiceInt(string title, IList<string> choices)
            => choices.IndexOf(MultipleChoice(title, choices));

        private static CardInstance MultipleChoiceCard(string title, IEnumerable<CardInstance> choices)
            => AnsiConsole.Prompt(
                new SelectionPrompt<CardInstance>()
                    .Title(title)
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(choices)
                    .AddChoice(null)
                    .UseConverter(card => card == null
                        ? "Back"
                        : $"[darkorange]{card.Base.Name}[/]"));

        private static CreatureCardInstance MultipleChoiceCreature(string title, IEnumerable<CreatureCardInstance> choices)
            => AnsiConsole.Prompt(
                new SelectionPrompt<CreatureCardInstance>()
                    .Title(title)
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(choices)
                    .AddChoice(null)
                    .UseConverter(card => card == null 
                        ? "Back"
                        : $"[darkorange]{card.Base.Name}[/] [red]{card.Health}[/] [dodgerblue1]{card.Attack}[/]"));
    }
}
