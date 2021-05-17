namespace CardGame.Client.Instances.Players
{
    public class OpponentInstance : PlayerInstance
    {
        /// <summary>
        /// The number of cards in the hand of your opponent.
        /// </summary>
        public int HandSize { get; set; }

        /// <summary>
        /// The number of cards in the deck of the opponent.
        /// </summary>
        public int DeckSize { get; set; } = 40;
    }
}
