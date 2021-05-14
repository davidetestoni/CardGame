using System;

namespace CardGame.Shared.Messages
{
    /// <summary>
    /// This is a message exchanged between client and server.
    /// </summary>
    public abstract class Message
    {
        public Guid Id { get; set; }

        public Message()
        {
            Id = Guid.NewGuid();
        }
    }
}
