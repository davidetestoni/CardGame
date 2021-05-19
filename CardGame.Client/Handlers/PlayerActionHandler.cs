using CardGame.Client.Models.PlayerActions;
using CardGame.Client.Models.PlayerActions.Cards.Creatures;
using CardGame.Client.Models.PlayerActions.Game;
using CardGame.Shared.Messages.Client;
using CardGame.Shared.Messages.Client.Cards.Creatures;
using CardGame.Shared.Messages.Client.Game;
using LiteNetLib;
using System;

namespace CardGame.Client.Handlers
{
    /// <summary>
    /// Takes player actions, turns them into messages of type <see cref="ClientMessage"/>
    /// and send them to the server.
    /// </summary>
    public class PlayerActionHandler
    {
        private readonly ClientMessageHandler clientMessageHandler;

        public PlayerActionHandler(ClientMessageHandler clientMessageHandler)
        {
            this.clientMessageHandler = clientMessageHandler;
        }

        /// <summary>
        /// Performs an <paramref name="action"/> by sending it to the server with the
        /// given <paramref name="deliveryMethod"/>.
        /// </summary>
        public void PerformAction(PlayerAction action, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            ClientMessage message;

            switch (action)
            {
                case EndTurnAction _:
                    message = new EndTurnMessage();
                    break;

                case SurrenderAction _:
                    message = new SurrenderMessage();
                    break;

                case AttackCreatureAction x:
                    message = new AttackCreatureMessage { AttackerId = x.Attacker.Id, TargetId = x.Target.Id };
                    break;

                case AttackPlayerAction x:
                    message = new AttackPlayerMessage { AttackerId = x.Attacker.Id };
                    break;

                case PlayCardAction x:
                    message = new PlayCardMessage { CardId = x.Card.Id };
                    break;

                default:
                    throw new NotImplementedException();
            }

            clientMessageHandler.SendMessage(message, deliveryMethod);
        }
    }
}
