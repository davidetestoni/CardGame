using Spectre.Console;
using System;

namespace SampleGame.Client.Console.Logging
{
    public static class Log
    {
        public static void Debug(string message)
            => AnsiConsole.MarkupLine("[grey][[{0}]] [[Debug]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void Info(string message)
            => AnsiConsole.MarkupLine("[white][[{0}]] [[Info]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void FormattedInfo(string message)
            => AnsiConsole.MarkupLine("[white][[{0}]] [[Info]] {1}[/]",
                DateTime.Now, message);

        public static void GameEvent(string message)
            => AnsiConsole.MarkupLine("[dodgerblue1][[{0}]] [[Game Event]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void FormattedGameEvent(string message)
            => AnsiConsole.MarkupLine("[dodgerblue1][[{0}]] [[Game Event]] {1}[/]",
                DateTime.Now, message);

        public static void Warning(string message)
            => AnsiConsole.MarkupLine("[darkorange][[{0}]] [[Warning]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void Error(string message)
            => AnsiConsole.MarkupLine("[red][[{0}]] [[Error]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void Exception(Exception ex)
            => AnsiConsole.WriteException(ex);

        public static void ClientMessage(string message)
            => AnsiConsole.MarkupLine("[turquoise2][[{0}]] [[Client -> Server]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void ServerMessage(string message)
           => AnsiConsole.MarkupLine("[darkolivegreen2][[{0}]] [[Server -> Client]] {1}[/]",
                DateTime.Now, Markup.Escape(message));
    }
}
