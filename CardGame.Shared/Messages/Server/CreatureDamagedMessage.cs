using System;

namespace CardGame.Shared.Messages.Server
{
    public class CreatureDamagedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the creature that took damage.
        /// </summary>
        Guid TargetId { get; set; }
    }
}
