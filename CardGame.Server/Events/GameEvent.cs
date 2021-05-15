using System;

namespace CardGame.Server.Events
{
    public class GameEvent
    {
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
