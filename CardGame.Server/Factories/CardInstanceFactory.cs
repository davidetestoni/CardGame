using CardGame.Server.Models.Cards;
using CardGame.Server.Models.Cards.Instances;
using System;

namespace CardGame.Server.Factories
{
    public class CardInstanceFactory
    {
        public static CardInstance Create(Card card, GameInstance game)
        {
            CardInstance instance = card switch
            {
                CreatureCard x => CreateCreature(x),
                _ => throw new NotImplementedException()
            };

            instance.Base = card;
            instance.Game = game;
            instance.Id = Guid.NewGuid();
            instance.ManaCost = card.ManaCost;

            return instance;
        }

        private static CreatureCardInstance CreateCreature(CreatureCard card)
        {
            return new CreatureCardInstance
            {
                Attack = card.Attack,
                Health = card.Health
            };
        }
    }
}
