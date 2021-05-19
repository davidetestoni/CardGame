using System;

namespace CardGame.Client.Models.PlayerActions
{
    /// <summary>
    /// Represents an action you can perform in the game.
    /// </summary>
    public abstract class PlayerAction
    {
        /// <summary>
        /// The time when the action was performed.
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
