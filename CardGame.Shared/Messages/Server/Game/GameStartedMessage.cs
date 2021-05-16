using CardGame.Shared.DTOs;
using System;
using System.Collections.Generic;

namespace CardGame.Shared.Messages.Server.Game
{
    public class GameStartedMessage : ServerMessage
    {
        /// <summary>
        /// The player that goes first.
        /// </summary>
        public Guid CurrentPlayerId { get; set; }

        /// <summary>
        /// The player ids and their info.
        /// </summary>
        public Dictionary<Guid, PlayerInfoDTO> Players { get; set; }
    }
}
