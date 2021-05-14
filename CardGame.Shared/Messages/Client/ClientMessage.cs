using System;
using Newtonsoft.Json;

namespace CardGame.Shared.Messages.Client
{
    /// <summary>
    /// This is a message sent from the client to the server.
    /// </summary>
    public abstract class ClientMessage
    {
        /// <summary>
        /// The unique id of the message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The id of the player that sent the message.
        /// </summary>
        [JsonIgnore]
        public Guid PlayerId { get; set; }
    }
}
