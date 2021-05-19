using CardGame.Client.Instances.Cards;

namespace CardGame.Client.Models.PlayerActions.Cards.Creatures
{
    /// <summary>
    /// Action used to play a card from your hand.
    /// </summary>
    public class PlayCardAction : PlayerAction
    {
        /// <summary>
        /// The card you want to play.
        /// </summary>
        public CardInstance Card { get; set; }
    }
}
