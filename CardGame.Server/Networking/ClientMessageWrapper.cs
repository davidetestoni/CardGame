using System;

namespace CardGame.Server.Networking
{
    /// <summary>
    /// Wrapper for raw client messages that keeps track of
    /// which client sent the message.
    /// </summary>
    public class ClientMessageWrapper
    {
        /// <summary>
        /// The raw body of the message.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// The id of the client that sent the message.
        /// </summary>
        public Guid SenderId { get; }

        public ClientMessageWrapper(string body, Guid senderId)
        {
            Body = body;
            SenderId = senderId;
        }
    }
}
