using System;

namespace CardGame.Shared.Messages.Server
{
    /// <summary>
    /// This is a message sent from the server to the client.
    /// </summary>
    public abstract class ServerMessage
    {
        /// <summary>
        /// The unique id of the message.
        /// </summary>
        public Guid Id { get; set; }
    }
}
