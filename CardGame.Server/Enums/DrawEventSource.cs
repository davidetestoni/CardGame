namespace CardGame.Server.Enums
{
    /// <summary>
    /// The source that caused cards to be drawn.
    /// </summary>
    public enum DrawEventSource
    {
        /// <summary>
        /// The game started.
        /// </summary>
        GameStart,

        /// <summary>
        /// A new turn started.
        /// </summary>
        TurnStart,

        /// <summary>
        /// The effect of a card.
        /// </summary>
        Effect
    }
}
