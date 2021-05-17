using System;

namespace CardGame.Client.Events
{
    public class GameEvent
    {
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
