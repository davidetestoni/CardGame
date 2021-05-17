using CardGame.Client.Instances.Game;
using CardGame.Shared.Messages.Server;
using CardGame.Shared.Messages.Server.Cards.Creatures;
using CardGame.Shared.Messages.Server.Game;
using CardGame.Shared.Messages.Server.Players;
using CardGame.Shared.Messages.Server.System;
using System;

namespace CardGame.Client.Handlers
{
    public class GameEventHandler
    {
        private readonly GameInstance game;
        
        public event EventHandler<string> Error;

        public GameEventHandler(GameInstance game, ServerMessageHandler serverMessageHandler)
        {
            serverMessageHandler.MessageReceived += (sender, message) => Handle(message);
            this.game = game;
        }

        private void Handle(ServerMessage message)
        {
            switch (message)
            {
                #region Game
                case GameStartedMessage x:
                    game.StartGame(x.OpponentId, x.OpponentInfo, x.MyTurn, x.Deck);
                    break;

                case GameEndedMessage x:
                    game.EndGame(x.WinnerId, x.Surrender);
                    break;

                case NewTurnMessage x:
                    game.ChangeTurn(x.CurrentPlayerId, x.TurnNumber);
                    break;

                case CardsDrawnMessage x:
                    game.DrawCards(x.NewCards);
                    break;

                case CardsDrawnOpponentMessage x:
                    game.DrawCardsOpponent(x.Amount);
                    break;
                #endregion

                #region Player
                case PlayerAttackedMessage x:
                    game.AttackPlayer(x.AttackerId, x.PlayerId, x.Damage);
                    break;

                case PlayerDamagedMessage x:
                    game.DamagePlayer(x.PlayerId, x.Damage);
                    break;

                case PlayerHealthRestoredMessage x:
                    game.RestorePlayerHealth(x.PlayerId, x.Amount);
                    break;

                case PlayerManaRestoredMessage x:
                    game.RestorePlayerMana(x.PlayerId, x.Amount);
                    break;

                case PlayerManaSpentMessage x:
                    game.SpendPlayerMana(x.PlayerId, x.Amount);
                    break;

                case PlayerMaxManaIncreasedMessage x:
                    game.IncreasePlayerMaxMana(x.PlayerId, x.Increment);
                    break;
                #endregion

                #region Creatures
                case CreatureAttackChangedMessage x:
                    game.ChangeCreatureAttack(x.CreatureId, x.NewValue);
                    break;

                case CreatureAttackedMessage x:
                    game.AttackCreature(x.AttackerId, x.DefenderId, x.Damage, x.RecoilDamage);
                    break;

                case CreatureAttacksLeftChangedMessage x:
                    game.ChangeCreatureAttacksLeft(x.CreatureId, x.CanAttack);
                    break;

                case CreatureDamagedMessage x:
                    game.DamageCreature(x.TargetId, x.Damage);
                    break;

                case CreatureDestroyedMessage x:
                    game.DestroyCreature(x.CreatureId);
                    break;

                case CreatureHealthIncreasedMessage x:
                    game.IncreaseCreatureHealth(x.CreatureId, x.Amount);
                    break;

                case CreaturePlayedMessage x:
                    game.PlayCreature(x.CreatureId);
                    break;

                case CreaturePlayedOpponentMessage x:
                    game.PlayCreatureOpponent(x.ShortName, x.CreatureId);
                    break;

                case CreatureSpawnedMessage x:
                    game.SpawnCreature(x.ShortName, x.CreatureId, x.PlayerId);
                    break;
                #endregion

                case ErrorResponse x:
                    Error?.Invoke(this, x.Error);
                    break;
            }
        }
    }
}
