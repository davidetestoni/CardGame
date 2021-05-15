using System;

namespace CardGame.Shared.Messages.Server.Game
{
    public class DrawnCardDTO
    {
        public string ShortName { get; set; }
        public Guid Id { get; set; }
    }
}
