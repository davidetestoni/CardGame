﻿using CardGame.Client.Instances.Players;

namespace CardGame.Client.Events.Players
{
    public class PlayerMaxManaIncreasedEvent : GameEvent
    {
        public PlayerInstance Player { get; set; }
        public int Increment { get; set; }
    }
}