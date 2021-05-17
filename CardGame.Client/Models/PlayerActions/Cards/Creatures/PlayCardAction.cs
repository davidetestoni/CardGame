using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Models.PlayerActions.Cards.Creatures
{
    public class PlayCardAction : PlayerAction
    {
        public CardInstance Card { get; set; }
    }
}
