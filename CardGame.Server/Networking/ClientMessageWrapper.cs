using System;

namespace CardGame.Server.Networking
{
    public class ClientMessageWrapper
    {
        public string Body { get; }
        public Guid SenderId { get; }

        public ClientMessageWrapper(string body, Guid senderId)
        {
            Body = body;
            SenderId = senderId;
        }
    }
}
