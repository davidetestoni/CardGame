using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Instances.Cards;
using CardGame.Shared.Messages.Client;
using System;
using CardGame.Shared.Messages.Server.System;
using CardGame.Shared.Messages.Client.Cards.Creatures;
using CardGame.Shared.Messages.Client.Game;

namespace CardGame.Server.Handlers
{
    public class PlayerActionHandler
    {
        private readonly GameInstance game;
        private readonly GameEventHandler gameEventHandler;

        public PlayerActionHandler(GameInstance game, ClientMessageHandler messageHandler,GameEventHandler gameEventHandler)
        {
            this.game = game;
            this.gameEventHandler = gameEventHandler;
            messageHandler.MessageReceived += HandleMessage;
        }

        private void HandleMessage(object sender, ClientMessage message)
        {
            var player = GetPlayer(message.PlayerId);

            try
            {
                switch (message)
                {
                    case AttackCreatureMessage x:
                        game.AttackCreature(player, GetCreatureOnField(x.AttackerId, player), GetCreatureOnField(x.TargetId, player));
                        break;

                    case AttackPlayerMessage x:
                        game.AttackPlayer(player, GetCreatureOnField(x.AttackerId, player));
                        break;

                    case PlayCardMessage x:
                        var cardToPlay = GetCardInHand(x.CardId, player);
                        switch (cardToPlay)
                        {
                            case CreatureCardInstance c:
                                game.PlayCreatureFromHand(player, c);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                        break;

                    case EndTurnMessage x:
                        game.EndTurn(player);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new ErrorResponse { Error = ex.Message };
                gameEventHandler.SendMessage(errorMessage, message.PlayerId);
            }
        }

        private PlayerInstance GetPlayer(Guid playerId)
        {
            if (game.PlayerOne.Id == playerId)
            {
                return game.PlayerOne;
            }
            else if (game.PlayerTwo.Id == playerId)
            {
                return game.PlayerTwo;
            }

            throw new Exception($"Player with id {playerId} not found");
        }

        private CreatureCardInstance GetCreatureOnField(Guid cardId, PlayerInstance player)
        {
            foreach (var card in player.Field)
            {
                if (card.Id == cardId)
                {
                    return card;
                }
            }

            throw new Exception($"Card with id {cardId} not found");
        }

        private CardInstance GetCardInHand(Guid cardId, PlayerInstance player)
        {
            foreach (var card in player.Field)
            {
                if (card.Id == cardId)
                {
                    return card;
                }
            }

            throw new Exception($"Card with id {cardId} not found");
        }
    }
}
