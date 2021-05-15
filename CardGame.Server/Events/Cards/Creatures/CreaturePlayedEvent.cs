﻿using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreaturePlayedEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public CreatureCardInstance Creature { get; set; }
    }
}
