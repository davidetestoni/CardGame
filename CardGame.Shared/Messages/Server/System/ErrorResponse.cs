namespace CardGame.Shared.Messages.Server.System
{
    public class ErrorResponse : ServerMessage
    {
        public string Error { get; set; }
    }
}
