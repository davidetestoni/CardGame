﻿using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Attributes;
using CardGame.Shared.Models.Cards;

namespace SampleGame.Cards.Creatures
{
    [PlayableCard(typeof(CreatureCardInstance)), Taunt]
    public class Defender : CreatureCard
    {
        public Defender()
        {
            ShortName = "Defender";
            Name = "Defender";
            Description = "Description of a defender";

            ManaCost = 3;
            Attack = 1;
            Health = 5;
        }
    }
}