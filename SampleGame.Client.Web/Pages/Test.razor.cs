using CardGame.Client.Factories;
using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;
using SampleGame.Cards.Creatures;
using System;
using System.Collections.Generic;

namespace SampleGame.Client.Web.Pages
{
    public partial class Test
    {
        public OpponentInstance Opponent { get; set; }
        public MeInstance Me { get; set; }

        private CardInstanceFactory factory;

        public Test()
        {
            factory = new CardInstanceFactory(typeof(Roland).Assembly);

            Opponent = new OpponentInstance
            {
                Name = "Player 2",
                CurrentHealth = 25,
                InitialHealth = 30,
                DeckSize = 57,
                CurrentMana = 3,
                MaximumMana = 5,
                HandSize = 9,
                Field = new List<CreatureCardInstance>
                {
                    NewCreature("Roland"),
                    NewCreature("Simon")
                },
                Graveyard = new List<CardInstance>
                {
                    NewCreature("Roland")
                }
            };

            Me = new MeInstance
            {
                Name = "Player 1",
                CurrentHealth = 18,
                InitialHealth = 30,
                CurrentMana = 1,
                MaximumMana = 6,
                Deck = new List<CardInstance>
                {
                    NewCreature("Adrian"),
                    NewCreature("Roland"),
                    NewCreature("Roland")
                },
                Hand = new List<CardInstance>
                {
                    NewCreature("Lizzie"),
                    NewCreature("Evie"),
                    NewCreature("Lenny")
                },
                Field = new List<CreatureCardInstance>
                {
                    NewCreature("Roland"),
                    NewCreature("Roland"),
                    NewCreature("Holly"),
                    NewCreature("Christie")
                },
                Graveyard = new List<CardInstance>
                {
                    NewCreature("Roland"),
                    NewCreature("Roland")
                }
            };
        }

        private CreatureCardInstance NewCreature(string shortName)
            => factory.Create(shortName, Guid.Empty, Opponent) as CreatureCardInstance;
    }
}
