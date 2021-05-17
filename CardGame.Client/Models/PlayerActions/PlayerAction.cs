using System;

namespace CardGame.Client.Models.PlayerActions
{
    public abstract class PlayerAction
    {
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
