﻿using CardGame.Server.Models.Cards.Instances;

namespace CardGame.Server.Events.Cards.Creatures
{
    public class CreatureHealthIncreasedEvent : GameEvent
    {
        public CreatureCardInstance Creature { get; set; }
        public int Amount { get; set; }
    }
}
