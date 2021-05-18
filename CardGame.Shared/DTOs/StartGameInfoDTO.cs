namespace CardGame.Shared.DTOs
{
    public class StartGameInfoDTO
    {
        /// <summary>
        /// Info about the opponent.
        /// </summary>
        public OpponentInfoDTO OpponentInfo { get; set; }

        /// <summary>
        /// The starting health of players.
        /// </summary>
        public int InitialHealth { get; set; }
    }
}
