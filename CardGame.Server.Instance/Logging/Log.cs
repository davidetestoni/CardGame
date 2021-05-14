using LiteNetLib;
using Spectre.Console;
using System;

namespace CardGame.Server.Instance.Logging
{
    public static class Log
    {
        public static void Info(string message)
            => AnsiConsole.MarkupLine("[white][[{0}]] [[Info]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void Warning(string message)
            => AnsiConsole.MarkupLine("[darkorange][[{0}]] [[Warning]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void Error(string message)
            => AnsiConsole.MarkupLine("[red][[{0}]] [[Error]] {1}[/]",
                DateTime.Now, Markup.Escape(message));

        public static void Exception(Exception ex)
            => AnsiConsole.WriteException(ex);

        public static void ClientMessage(string message, NetPeer client)
            => AnsiConsole.MarkupLine("[turquoise2][[{0}]] [[{1} -> Server]] {2}[/]",
                DateTime.Now, client.EndPoint, message);

        public static void ServerMessage(string message, NetPeer client)
           => AnsiConsole.MarkupLine("[darkolivegreen2][[{0}]] [[Server -> {1}]] {2}[/]",
                DateTime.Now, client.EndPoint, message);
    }
}
