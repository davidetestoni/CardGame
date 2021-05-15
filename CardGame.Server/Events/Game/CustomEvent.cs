namespace CardGame.Server.Events.Game
{
    public class CustomEvent : GameEvent
    {
        public string ShortName { get; set; }
        public object Data { get; set; }
    }
}
