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
    /// <summary>
    /// Handles actions received from clients and tries to apply them to a <see cref="GameInstance"/>.
    /// </summary>
    public class PlayerActionHandler
    {
        private readonly GameInstance game;
        private readonly ServerMessageHandler serverMessageHandler;

        public PlayerActionHandler(GameInstance game, ClientMessageHandler messageHandler, ServerMessageHandler serverMessageHandler)
        {
            this.game = game;
            this.serverMessageHandler = serverMessageHandler;
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
                        game.AttackCreature(player, GetCreatureOnField(x.AttackerId, player), GetCreatureOnField(x.TargetId, game.GetOpponent(player)));
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

                    case SurrenderMessage x:
                        game.Surrender(player);
                        break;

                    default:
                        break; // Not a game-related message, we can ignore it
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new ErrorResponse { Error = ex.Message };
                serverMessageHandler.SendMessage(errorMessage, message.PlayerId);
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
            foreach (var card in player.Hand)
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
