using System;

namespace CardGame.Shared.DTOs
{
    public class CardInfoDTO
    {
        /// <summary>
        /// The shortname of the card.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The unique id of the card.
        /// </summary>
        public Guid Id { get; set; }
    }
}
