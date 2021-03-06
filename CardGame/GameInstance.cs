namespace CardGame
{
    public class GameInstance
    {
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public int TurnNumber { get; set; } = 1;
        public Player CurrentPlayer { get; set; }
        public Player Opponent => PlayerOne != CurrentPlayer ? PlayerOne : PlayerTwo;

        public bool IsOver { get; set; } = false;
        public Player Winner { get; set; } = null;
    }
}
