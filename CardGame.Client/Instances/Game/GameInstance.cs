using CardGame.Client.Instances.Players;
using CardGame.Shared.Enums;
using System;

namespace CardGame.Client.Instances.Game
{
    public class GameInstance
    {
        public Guid Id { get; set; }

        public PlayerInstance Me { get; set; }
        public PlayerInstance Opponent { get; set; }

        public int TurnNumber { get; set; } = 1;
        public bool MyTurn { get; set; } = false;

        public GameStatus Status { get; set; } = GameStatus.Created;
        public PlayerInstance Winner { get; set; } = null;
    }
}
