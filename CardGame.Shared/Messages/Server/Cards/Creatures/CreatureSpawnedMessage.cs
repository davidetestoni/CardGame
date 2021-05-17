using System;

namespace CardGame.Shared.Messages.Server.Cards.Creatures
{
    public class CreatureSpawnedMessage : ServerMessage
    {
        /// <summary>
        /// The id of the card that was spawned.
        /// </summary>
        public Guid CreatureId { get; set; }

        /// <summary>
        /// The shortname of the card that was spawned.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The id of the owner of the newly spawned card.
        /// </summary>
        public Guid PlayerId { get; set; }
    }
}
