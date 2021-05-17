using CardGame.Client.Events.Cards.Creatures;
using CardGame.Client.Events.Game;
using CardGame.Client.Events.Players;
using CardGame.Client.Factories;
using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;
using CardGame.Shared.DTOs;
using CardGame.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame.Client.Instances.Game
{
    public class GameInstance
    {
        private readonly CardInstanceFactory cardInstanceFactory;

        public Guid Id { get; set; }

        public MeInstance Me { get; set; }
        public OpponentInstance Opponent { get; set; }

        public int TurnNumber { get; set; } = 1;
        public bool MyTurn { get; set; } = false;

        public GameStatus Status { get; set; } = GameStatus.Created;
        public PlayerInstance Winner { get; set; } = null;
        public bool Surrender { get; set; } = false;

        #region Events
        public event EventHandler<GameStartedEvent> GameStarted;
        public event EventHandler<GameEndedEvent> GameEnded;
        public event EventHandler<NewTurnEvent> NewTurn;
        public event EventHandler<CardsDrawnEvent> CardsDrawn;
        public event EventHandler<CardsDrawnOpponentEvent> CardsDrawnOpponent;

        public event EventHandler<PlayerAttackedEvent> PlayerAttacked;
        public event EventHandler<PlayerDamagedEvent> PlayerDamaged;
        public event EventHandler<PlayerHealthRestoredEvent> PlayerHealthRestored;
        public event EventHandler<PlayerManaRestoredEvent> PlayerManaRestored;
        public event EventHandler<PlayerManaSpentEvent> PlayerManaSpent;
        public event EventHandler<PlayerMaxManaIncreasedEvent> PlayerMaxManaIncreased;

        public event EventHandler<CreatureAttackChangedEvent> CreatureAttackChanged;
        public event EventHandler<CreatureAttackedEvent> CreatureAttacked;
        public event EventHandler<CreatureAttacksLeftChangedEvent> CreatureAttacksLeftChanged;
        public event EventHandler<CreatureDamagedEvent> CreatureDamaged;
        public event EventHandler<CreatureDestroyedEvent> CreatureDestroyed;
        public event EventHandler<CreatureHealthIncreasedEvent> CreatureHealthIncreased;
        public event EventHandler<CreaturePlayedEvent> CreaturePlayed;
        public event EventHandler<CreatureSpawnedEvent> CreatureSpawned;
        #endregion

        public GameInstance(CardInstanceFactory cardInstanceFactory)
        {
            this.cardInstanceFactory = cardInstanceFactory;
        }

        public PlayerInstance GetPlayer(Guid id)
        {
            if (Me.Id == id)
            {
                return Me;
            }
            else
            {
                return Opponent;
            }
        }

        #region Game
        public void StartGame(Guid opponentId, OpponentInfoDTO opponentInfo, bool myTurn, List<CardInfoDTO> deck)
        {
            Opponent = new OpponentInstance
            {
                Id = opponentId,
                Name = opponentInfo.Name,
                DeckSize = opponentInfo.DeckSize
            };

            MyTurn = myTurn;
            Me.Deck = ConvertDeck(deck);

            Status = GameStatus.Started;

            PlayerInstance currentPlayer;

            if (myTurn)
            {
                currentPlayer = Me;
            }
            else
            {
                currentPlayer = Opponent;
            }

            GameStarted?.Invoke(this, new GameStartedEvent
            {
                CurrentPlayer = currentPlayer
            });
        }

        public void EndGame(Guid winnerId, bool surrender)
        {
            Winner = GetPlayer(winnerId);
            Surrender = surrender;
            Status = GameStatus.Finished;

            GameEnded?.Invoke(this, new GameEndedEvent
            {
                Winner = Winner,
                Surrender = Surrender
            });
        }

        public void ChangeTurn(Guid currentPlayerId, int turnNumber)
        {
            MyTurn = currentPlayerId == Me.Id;
            TurnNumber = turnNumber;

            NewTurn?.Invoke(this, new NewTurnEvent
            {
                CurrentPlayer = GetPlayer(currentPlayerId),
                TurnNumber = TurnNumber
            });
        }

        public void DrawCards(List<Guid> newCards)
        {
            var instances = new List<CardInstance>();

            foreach (var card in newCards)
            {
                var instance = GetCardInDeck(card);
                instances.Add(instance);
                Me.Deck.Remove(instance);
            }

            Me.Hand.AddRange(instances);
            CardsDrawn?.Invoke(this, new CardsDrawnEvent
            {
                NewCards = instances
            });
        }

        public void DrawCardsOpponent(int amount)
        {
            Opponent.HandSize += amount;
            Opponent.DeckSize -= amount;

            CardsDrawnOpponent?.Invoke(this, new CardsDrawnOpponentEvent
            {
                Amount = amount
            });
        }
        #endregion

        #region Players
        public void AttackPlayer(Guid attackerId, Guid playerId, int damage)
        {
            GetPlayer(playerId).CurrentHealth -= damage;

            PlayerAttacked?.Invoke(this, new PlayerAttackedEvent
            {
                Attacker = GetCreatureOnField(attackerId),
                Player = GetPlayer(playerId),
                Damage = damage
            });
        }

        public void DamagePlayer(Guid playerId, int damage)
        {
            GetPlayer(playerId).CurrentHealth -= damage;

            PlayerDamaged?.Invoke(this, new PlayerDamagedEvent
            {
                Player = GetPlayer(playerId),
                Damage = damage
            });
        }

        public void RestorePlayerHealth(Guid playerId, int amount)
        {
            GetPlayer(playerId).CurrentHealth += amount;

            PlayerHealthRestored?.Invoke(this, new PlayerHealthRestoredEvent
            {
                Player = GetPlayer(playerId),
                Amount = amount
            });
        }

        public void RestorePlayerMana(Guid playerId, int amount)
        {
            GetPlayer(playerId).CurrentMana += amount;

            PlayerManaRestored?.Invoke(this, new PlayerManaRestoredEvent
            {
                Player = GetPlayer(playerId),
                Amount = amount
            });
        }

        public void SpendPlayerMana(Guid playerId, int amount)
        {
            GetPlayer(playerId).CurrentMana -= amount;

            PlayerManaSpent?.Invoke(this, new PlayerManaSpentEvent
            {
                Player = GetPlayer(playerId),
                Amount = amount
            });
        }

        public void IncreasePlayerMaxMana(Guid playerId, int increment)
        {
            GetPlayer(playerId).MaximumMana += increment;

            PlayerMaxManaIncreased?.Invoke(this, new PlayerMaxManaIncreasedEvent
            {
                Player = GetPlayer(playerId),
                Increment = increment
            });
        }
        #endregion

        #region Creatures
        public void ChangeCreatureAttack(Guid creatureId, int newValue)
        {
            var creature = GetCreatureOnField(creatureId);
            var oldValue = creature.Attack;
            creature.Attack = newValue;

            CreatureAttackChanged?.Invoke(this, new CreatureAttackChangedEvent
            {
                Creature = creature,
                OldValue = oldValue,
                NewValue = newValue
            });
        }

        public void AttackCreature(Guid attackedId, Guid defenderId, int damage, int recoilDamage)
        {
            var attacker = GetCreatureOnField(attackedId);
            var defender = GetCreatureOnField(defenderId);
            attacker.Health -= recoilDamage;
            defender.Health -= damage;

            CreatureAttacked?.Invoke(this, new CreatureAttackedEvent
            {
                Attacker = attacker,
                Defender = defender,
                Damage = damage,
                RecoilDamage = recoilDamage
            });
        }

        public void ChangeCreatureAttacksLeft(Guid creatureId, bool canAttack)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.CanAttack = canAttack;

            CreatureAttacksLeftChanged?.Invoke(this, new CreatureAttacksLeftChangedEvent
            {
                Creature = creature,
                CanAttack = canAttack
            });
        }

        public void DamageCreature(Guid creatureId, int damage)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.Health -= damage;

            CreatureDamaged?.Invoke(this, new CreatureDamagedEvent
            {
                Target = creature,
                Damage = damage
            });
        }

        public void DestroyCreature(Guid creatureId)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.Owner.Field.Remove(creature);
            creature.Owner.Graveyard.Add(creature.Base);

            CreatureDestroyed?.Invoke(this, new CreatureDestroyedEvent
            {
                Target = creature
            });
        }

        public void IncreaseCreatureHealth(Guid creatureId, int amount)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.Health += amount;

            CreatureHealthIncreased?.Invoke(this, new CreatureHealthIncreasedEvent
            {
                Creature = creature,
                Amount = amount
            });
        }

        public void PlayCreature(Guid creatureId)
        {
            var creature = GetCardInHand(creatureId) as CreatureCardInstance;
            Me.Hand.Remove(creature);
            Me.Field.Add(creature);

            CreaturePlayed?.Invoke(this, new CreaturePlayedEvent
            {
                Creature = creature,
                Player = Me
            });
        }

        public void PlayCreatureOpponent(string shortName, Guid creatureId)
        {
            var creature = cardInstanceFactory.Create(shortName, creatureId) as CreatureCardInstance;
            Opponent.HandSize--;
            Opponent.Field.Add(creature);

            CreaturePlayed?.Invoke(this, new CreaturePlayedEvent
            {
                Creature = creature,
                Player = Opponent
            });
        }

        public void SpawnCreature(string shortName, Guid creatureId, Guid playerId)
        {
            var creature = cardInstanceFactory.Create(shortName, creatureId) as CreatureCardInstance;
            GetPlayer(playerId).Field.Add(creature);

            CreatureSpawned?.Invoke(this, new CreatureSpawnedEvent
            {
                Creature = creature
            });
        }
        #endregion

        private CreatureCardInstance GetCreatureOnField(Guid id)
        {
            var creature = Me.Field.FirstOrDefault(c => c.Id == id);

            if (creature != null)
            {
                return creature;
            }

            creature = Opponent.Field.First(c => c.Id == id);

            if (creature == null)
            {
                throw new Exception($"No creature found with id {id}");
            }

            return creature;
        }

        private CardInstance GetCardInHand(Guid id)
        {
            var card = Me.Hand.FirstOrDefault(c => c.Id == id);

            if (card == null)
            {
                throw new Exception($"No card found with id {id}");
            }

            return card;
        }

        private CardInstance GetCardInDeck(Guid id)
        {
            var card = Me.Deck.FirstOrDefault(c => c.Id == id);

            if (card == null)
            {
                throw new Exception($"No card found with id {id}");
            }

            return card;
        }

        private List<CardInstance> ConvertDeck(List<CardInfoDTO> cards)
        {
            var deck = new List<CardInstance>();

            foreach (var card in cards)
            {
                var instance = cardInstanceFactory.Create(card.ShortName, card.Id);
                deck.Add(instance);
            }

            return deck;
        }
    }
}
