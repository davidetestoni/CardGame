using CardGame.Server.Instances.Game;
using CardGame.Server.Instances.Players;
using CardGame.Server.Models.Cards.Instances;
using CardGame.Shared.Messages.Client;
using System;

namespace CardGame.Server.Handlers
{
    public class PlayerActionHandler
    {
        private readonly GameInstance game;

        public PlayerActionHandler(GameInstance game, ClientMessageHandler messageHandler)
        {
            this.game = game;
            messageHandler.MessageReceived += HandleMessage;
        }

        private void HandleMessage(object sender, ClientMessage message)
        {
            var player = GetPlayer(message.PlayerId);

            switch (message)
            {
                case AttackCreatureMessage x:
                    game.AttackCreature(player, GetCreatureOnField(x.AttackerId, player), GetCreatureOnField(x.TargetId, player));
                    break;
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
