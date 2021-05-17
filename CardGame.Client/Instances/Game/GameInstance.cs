using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;
using CardGame.Shared.Enums;
using System;
using System.Linq;

namespace CardGame.Client.Instances.Game
{
    public class GameInstance
    {
        public Guid Id { get; set; }

        public MeInstance Me { get; set; }
        public OpponentInstance Opponent { get; set; }

        public int TurnNumber { get; set; } = 1;
        public bool MyTurn { get; set; } = false;

        public GameStatus Status { get; set; } = GameStatus.Created;
        public PlayerInstance Winner { get; set; } = null;
        public bool Surrendered { get; set; } = false;

        public PlayerInstance GetPlayer(Guid id)
        {
            if (Me.Id == id)
            {
                return Me;
            }
            else
            {
                return Opponent;
            }
        }

        public CreatureCardInstance GetCreatureOnField(Guid id)
        {
            var creature = Me.Field.FirstOrDefault(c => c.Id == id);

            if (creature != null)
            {
                return creature;
            }

            return Opponent.Field.First(c => c.Id == id);
        }

        public CardInstance GetCardInHand(Guid id)
            => Me.Hand.FirstOrDefault(c => c.Id == id);
    }
}
