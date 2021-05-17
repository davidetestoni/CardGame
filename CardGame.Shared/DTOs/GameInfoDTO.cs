using System;

namespace CardGame.Shared.DTOs
{
    public class GameInfoDTO
    {
        public Guid Id { get; set; }
        public int InitialHealth { get; set; }
        public int FieldSize { get; set; }
    }
}
