using System;

namespace CardGame.Server.Networking
{
    public class ClientMessageWrapper
    {
        public string Body { get; init; }
        public Guid SenderId { get; init; }

        public ClientMessageWrapper(string body, Guid senderId)
        {
            Body = body;
            SenderId = senderId;
        }
    }
}
