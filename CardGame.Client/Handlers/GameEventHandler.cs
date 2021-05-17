using CardGame.Client.Factories;
using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Game;
using CardGame.Client.Instances.Players;
using CardGame.Shared.Enums;
using CardGame.Shared.Messages.Server;
using CardGame.Shared.Messages.Server.Cards.Creatures;
using CardGame.Shared.Messages.Server.Game;
using CardGame.Shared.Messages.Server.Players;
using System.Linq;

namespace CardGame.Client.Handlers
{
    public class GameEventHandler
    {
        private readonly GameInstance game;
        private readonly CardInstanceFactory cardInstanceFactory;

        public GameEventHandler(GameInstance game, CardInstanceFactory cardInstanceFactory, ServerMessageHandler serverMessageHandler)
        {
            serverMessageHandler.MessageReceived += (sender, message) => Handle(message);
            this.game = game;
            this.cardInstanceFactory = cardInstanceFactory;
        }

        private void Handle(ServerMessage message)
        {
            switch (message)
            {
                #region Game
                case GameStartedMessage x:
                    var opponent = x.Players.First(p => p.Key != game.Me.Id);
                    game.Opponent.Id = opponent.Key;
                    game.Opponent.Name = opponent.Value.Name;
                    game.MyTurn = x.CurrentPlayerId == game.Me.Id;
                    game.Status = GameStatus.Started;
                    break;

                case GameEndedMessage x:
                    game.Winner = game.GetPlayer(x.WinnerId);
                    game.Surrendered = x.Surrender;
                    break;

                case NewTurnMessage x:
                    game.MyTurn = x.CurrentPlayerId == game.Me.Id;
                    game.TurnNumber = x.TurnNumber;
                    break;

                case CardsDrawnMessage x:
                    foreach (var card in x.NewCards)
                    {
                        var instance = cardInstanceFactory.Create(card.ShortName, card.Id);
                        game.Me.Hand.Add(instance);
                    }
                    break;

                case CardsDrawnOpponentMessage x:
                    game.Opponent.HandSize += x.Amount;
                    game.Opponent.DeckSize = x.DeckSize;
                    break;
                #endregion

                #region Player
                case PlayerAttackedMessage x:
                    game.GetPlayer(x.PlayerId).CurrentHealth -= x.Damage;
                    break;

                case PlayerDamagedMessage x:
                    game.GetPlayer(x.PlayerId).CurrentHealth -= x.Damage;
                    break;

                case PlayerHealthRestoredMessage x:
                    game.GetPlayer(x.PlayerId).CurrentHealth += x.Amount;
                    break;

                case PlayerManaRestoredMessage x:
                    game.GetPlayer(x.PlayerId).CurrentMana += x.Amount;
                    break;

                case PlayerManaSpentMessage x:
                    game.GetPlayer(x.PlayerId).CurrentMana -= x.Amount;
                    break;

                case PlayerMaxManaIncreasedMessage x:
                    game.GetPlayer(x.PlayerId).MaximumMana += x.Increment;
                    break;
                #endregion

                #region Creatures
                case CreatureAttackedMessage x:
                    game.GetCreatureOnField(x.AttackerId).Health -= x.RecoilDamage;
                    game.GetCreatureOnField(x.DefenderId).Health -= x.Damage;
                    break;

                case CreatureAttackChangedMessage x:
                    game.GetCreatureOnField(x.CreatureId).Attack = x.NewValue;
                    break;

                case CreatureAttacksLeftChangedMessage x:
                    game.GetCreatureOnField(x.CreatureId).CanAttack = x.CanAttack;
                    break;

                case CreatureDamagedMessage x:
                    game.GetCreatureOnField(x.TargetId).Health -= x.Damage;
                    break;

                case CreatureDestroyedMessage x:
                    var creatureToDestroy = game.GetCreatureOnField(x.CreatureId);
                    creatureToDestroy.Owner.Field.Remove(creatureToDestroy);
                    creatureToDestroy.Owner.Graveyard.Add(creatureToDestroy.Base);
                    break;

                case CreatureHealthIncreasedMessage x:
                    game.GetCreatureOnField(x.CreatureId).Health += x.Amount;
                    break;

                case CreaturePlayedMessage x:
                    var creature = game.GetCardInHand(x.CreatureId) as CreatureCardInstance;
                    game.Me.Hand.Remove(creature);
                    game.Me.Field.Add(creature);
                    break;

                case CreaturePlayedOpponentMessage x:
                    creature = cardInstanceFactory.Create(x.ShortName, x.CreatureId) as CreatureCardInstance;
                    game.Opponent.HandSize--;
                    game.Opponent.Field.Add(creature);
                    break;

                case CreatureSpawnedMessage x:
                    creature = cardInstanceFactory.Create(x.ShortName, x.CreatureId) as CreatureCardInstance;
                    game.GetPlayer(x.PlayerId).Field.Add(creature);
                    break;
                #endregion
            }
        }
    }
}
