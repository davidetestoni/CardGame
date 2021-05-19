namespace CardGame.Server.Events.Game
{
    public class CustomEventArgs : GameEventArgs
    {
        public string ShortName { get; set; }
        public object Data { get; set; }
    }
}
