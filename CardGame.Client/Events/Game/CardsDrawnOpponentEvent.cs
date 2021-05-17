namespace CardGame.Client.Events.Game
{
    public class CardsDrawnOpponentEvent : GameEvent
    {
        public int Amount { get; set; }
    }
}
