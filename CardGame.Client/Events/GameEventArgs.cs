using System;

namespace CardGame.Client.Events
{
    /// <summary>
    /// Data of a game event.
    /// </summary>
    public class GameEventArgs : EventArgs
    {
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
