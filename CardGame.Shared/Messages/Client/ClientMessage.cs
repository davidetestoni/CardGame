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
        /// The id of the player that sent the message.
        /// Do not set this in the client, the server will take care of it.
        /// </summary>
        [JsonIgnore]
        public Guid PlayerId { get; set; }
    }
}
