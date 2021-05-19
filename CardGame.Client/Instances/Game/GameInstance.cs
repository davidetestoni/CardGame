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
    /// <summary>
    /// Client-side instance of the game, synchronized with the server
    /// through messaging and events.
    /// </summary>
    public class GameInstance
    {
        private readonly CardInstanceFactory cardInstanceFactory;

        /// <summary>
        /// The id of the game.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// You.
        /// </summary>
        public MeInstance Me { get; set; }

        /// <summary>
        /// Your opponent.
        /// </summary>
        public OpponentInstance Opponent { get; set; }

        /// <summary>
        /// How many turns have passed since the start of the game.
        /// </summary>
        public int TurnNumber { get; set; } = 1;

        /// <summary>
        /// Whether it's your turn to play.
        /// </summary>
        public bool MyTurn { get; set; } = false;

        /// <summary>
        /// The status of the game.
        /// </summary>
        public GameStatus Status { get; set; } = GameStatus.Created;

        /// <summary>
        /// The winner of the game (if the game hasn't ended yet, the winner is null).
        /// </summary>
        public PlayerInstance Winner { get; set; } = null;

        /// <summary>
        /// Whether the game finished because of a surrender from either player.
        /// </summary>
        public bool Surrendered { get; set; } = false;

        #region Events
        /// <summary>
        /// Called when the game starts.
        /// </summary>
        public event EventHandler<GameStartedEventArgs> GameStarted;

        /// <summary>
        /// Called when the game ends.
        /// </summary>
        public event EventHandler<GameEndedEventArgs> GameEnded;

        /// <summary>
        /// Called when the current turn ended and a new turn begins.
        /// </summary>
        public event EventHandler<NewTurnEventArgs> NewTurn;

        /// <summary>
        /// Called when you drew cards.
        /// </summary>
        public event EventHandler<CardsDrawnEventArgs> CardsDrawn;

        /// <summary>
        /// Called when your opponent drew cards.
        /// </summary>
        public event EventHandler<CardsDrawnOpponentEventArgs> CardsDrawnOpponent;

        /// <summary>
        /// Called when a player is attacked by a creature.
        /// </summary>
        public event EventHandler<PlayerAttackedEventArgs> PlayerAttacked;

        /// <summary>
        /// Called when a player is damaged by an effect.
        /// </summary>
        public event EventHandler<PlayerDamagedEventArgs> PlayerDamaged;

        /// <summary>
        /// Called when a player's health is restored.
        /// </summary>
        public event EventHandler<PlayerHealthRestoredEventArgs> PlayerHealthRestored;

        /// <summary>
        /// Called when a player's mana is restored.
        /// </summary>
        public event EventHandler<PlayerManaRestoredEventArgs> PlayerManaRestored;

        /// <summary>
        /// Called when a player spends mana.
        /// </summary>
        public event EventHandler<PlayerManaSpentEventArgs> PlayerManaSpent;

        /// <summary>
        /// Called when a player's maximum mana is increased.
        /// </summary>
        public event EventHandler<PlayerMaxManaIncreasedEventArgs> PlayerMaxManaIncreased;

        /// <summary>
        /// Called when the attack of a creature changed.
        /// </summary>
        public event EventHandler<CreatureAttackChangedEventArgs> CreatureAttackChanged;

        /// <summary>
        /// Called when a creature is attacked by another creature.
        /// </summary>
        public event EventHandler<CreatureAttackedEventArgs> CreatureAttacked;

        /// <summary>
        /// Called when the number of times a creature can attack during this turn is changed.
        /// </summary>
        public event EventHandler<CreatureAttacksLeftChangedEventArgs> CreatureAttacksLeftChanged;

        /// <summary>
        /// Called when a creature is damaged by an effect.
        /// </summary>
        public event EventHandler<CreatureDamagedEventArgs> CreatureDamaged;

        /// <summary>
        /// Called when a creature is destroyed.
        /// </summary>
        public event EventHandler<CreatureDestroyedEventArgs> CreatureDestroyed;

        /// <summary>
        /// Called when a creature's health is increased.
        /// </summary>
        public event EventHandler<CreatureHealthIncreasedEventArgs> CreatureHealthIncreased;
        
        /// <summary>
        /// Called when a creature is played from a player's hand.
        /// </summary>
        public event EventHandler<CreaturePlayedEventArgs> CreaturePlayed;

        /// <summary>
        /// Called when a creature is spawned on the field.
        /// </summary>
        public event EventHandler<CreatureSpawnedEventArgs> CreatureSpawned;
        #endregion

        public GameInstance(CardInstanceFactory cardInstanceFactory)
        {
            this.cardInstanceFactory = cardInstanceFactory;
        }

        /// <summary>
        /// Gets a player from its <paramref name="id"/>.
        /// </summary>
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
        internal void StartGame(StartGameInfoDTO gameInfo, bool myTurn, List<CardInfoDTO> deck)
        {
            Opponent = new OpponentInstance
            {
                Id = gameInfo.OpponentInfo.Id,
                Name = gameInfo.OpponentInfo.Name,
                DeckSize = gameInfo.OpponentInfo.DeckSize,
                InitialHealth = gameInfo.InitialHealth,
                CurrentHealth = gameInfo.InitialHealth
            };

            MyTurn = myTurn;
            Me.InitialHealth = gameInfo.InitialHealth;
            Me.CurrentHealth = gameInfo.InitialHealth;
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

            GameStarted?.Invoke(this, new GameStartedEventArgs
            {
                CurrentPlayer = currentPlayer
            });
        }

        internal void EndGame(Guid winnerId, bool surrender)
        {
            Winner = GetPlayer(winnerId);
            Surrendered = surrender;
            Status = GameStatus.Finished;

            GameEnded?.Invoke(this, new GameEndedEventArgs
            {
                Winner = Winner,
                Surrendered = Surrendered
            });
        }

        internal void ChangeTurn(Guid currentPlayerId, int turnNumber)
        {
            MyTurn = currentPlayerId == Me.Id;
            TurnNumber = turnNumber;

            NewTurn?.Invoke(this, new NewTurnEventArgs
            {
                CurrentPlayer = GetPlayer(currentPlayerId),
                TurnNumber = TurnNumber
            });
        }

        internal void DrawCards(List<Guid> newCards, List<Guid> destroyed)
        {
            var instances = new List<CardInstance>();
            var destroyedInstances = new List<CardInstance>();

            foreach (var card in newCards)
            {
                var instance = GetCardInDeck(card);
                instances.Add(instance);
                Me.Deck.Remove(instance);
            }

            Me.Hand.AddRange(instances);

            foreach (var card in destroyed)
            {
                var instance = GetCardInDeck(card);
                destroyedInstances.Add(instance);
                Me.Deck.Remove(instance);
            }

            Me.Graveyard.AddRange(destroyedInstances);

            CardsDrawn?.Invoke(this, new CardsDrawnEventArgs
            {
                NewCards = instances,
                Destroyed = destroyedInstances
            });
        }

        internal void DrawCardsOpponent(int amount, List<CardInfoDTO> destroyed)
        {
            var destroyedInstances = new List<CardInstance>();

            Opponent.HandSize += amount;
            Opponent.DeckSize -= amount;

            foreach (var card in destroyed)
            {
                var instance = cardInstanceFactory.Create(card.ShortName, card.Id, Opponent);
                destroyedInstances.Add(instance);
                Opponent.DeckSize--;
            }

            CardsDrawnOpponent?.Invoke(this, new CardsDrawnOpponentEventArgs
            {
                Amount = amount,
                Destroyed = destroyedInstances
            });
        }
        #endregion

        #region Players
        internal void AttackPlayer(Guid attackerId, Guid playerId, int damage)
        {
            GetPlayer(playerId).CurrentHealth -= damage;

            PlayerAttacked?.Invoke(this, new PlayerAttackedEventArgs
            {
                Attacker = GetCreatureOnField(attackerId),
                Player = GetPlayer(playerId),
                Damage = damage
            });
        }

        internal void DamagePlayer(Guid playerId, int damage)
        {
            GetPlayer(playerId).CurrentHealth -= damage;

            PlayerDamaged?.Invoke(this, new PlayerDamagedEventArgs
            {
                Player = GetPlayer(playerId),
                Damage = damage
            });
        }

        internal void RestorePlayerHealth(Guid playerId, int amount)
        {
            GetPlayer(playerId).CurrentHealth += amount;

            PlayerHealthRestored?.Invoke(this, new PlayerHealthRestoredEventArgs
            {
                Player = GetPlayer(playerId),
                Amount = amount
            });
        }

        internal void RestorePlayerMana(Guid playerId, int amount)
        {
            GetPlayer(playerId).CurrentMana += amount;

            PlayerManaRestored?.Invoke(this, new PlayerManaRestoredEventArgs
            {
                Player = GetPlayer(playerId),
                Amount = amount
            });
        }

        internal void SpendPlayerMana(Guid playerId, int amount)
        {
            GetPlayer(playerId).CurrentMana -= amount;

            PlayerManaSpent?.Invoke(this, new PlayerManaSpentEventArgs
            {
                Player = GetPlayer(playerId),
                Amount = amount
            });
        }

        internal void IncreasePlayerMaxMana(Guid playerId, int increment)
        {
            GetPlayer(playerId).MaximumMana += increment;

            PlayerMaxManaIncreased?.Invoke(this, new PlayerMaxManaIncreasedEventArgs
            {
                Player = GetPlayer(playerId),
                Increment = increment
            });
        }
        #endregion

        #region Creatures
        internal void ChangeCreatureAttack(Guid creatureId, int amount)
        {
            var creature = GetCreatureOnField(creatureId);
            var oldValue = creature.Attack;
            creature.Attack += amount;

            CreatureAttackChanged?.Invoke(this, new CreatureAttackChangedEventArgs
            {
                Creature = creature,
                Amount = amount,
            });
        }

        internal void AttackCreature(Guid attackedId, Guid defenderId, int damage, int recoilDamage)
        {
            var attacker = GetCreatureOnField(attackedId);
            var defender = GetCreatureOnField(defenderId);
            attacker.Health -= recoilDamage;
            defender.Health -= damage;

            CreatureAttacked?.Invoke(this, new CreatureAttackedEventArgs
            {
                Attacker = attacker,
                Defender = defender,
                Damage = damage,
                RecoilDamage = recoilDamage
            });
        }

        internal void ChangeCreatureAttacksLeft(Guid creatureId, bool canAttack)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.CanAttack = canAttack;

            CreatureAttacksLeftChanged?.Invoke(this, new CreatureAttacksLeftChangedEventArgs
            {
                Creature = creature,
                CanAttack = canAttack
            });
        }

        internal void DamageCreature(Guid creatureId, int damage)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.Health -= damage;

            CreatureDamaged?.Invoke(this, new CreatureDamagedEventArgs
            {
                Target = creature,
                Damage = damage
            });
        }

        internal void DestroyCreature(Guid creatureId)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.Owner.Field.Remove(creature);
            creature.Owner.Graveyard.Add(creature);

            CreatureDestroyed?.Invoke(this, new CreatureDestroyedEventArgs
            {
                Target = creature
            });
        }

        internal void IncreaseCreatureHealth(Guid creatureId, int amount)
        {
            var creature = GetCreatureOnField(creatureId);
            creature.Health += amount;

            CreatureHealthIncreased?.Invoke(this, new CreatureHealthIncreasedEventArgs
            {
                Creature = creature,
                Amount = amount
            });
        }

        internal void PlayCreature(Guid creatureId)
        {
            var creature = GetCardInHand(creatureId) as CreatureCardInstance;
            Me.Hand.Remove(creature);
            Me.Field.Add(creature);

            CreaturePlayed?.Invoke(this, new CreaturePlayedEventArgs
            {
                Creature = creature,
                Player = Me
            });
        }

        internal void PlayCreatureOpponent(string shortName, Guid creatureId)
        {
            var creature = cardInstanceFactory.Create(shortName, creatureId, Opponent) as CreatureCardInstance;
            Opponent.HandSize--;
            Opponent.Field.Add(creature);

            CreaturePlayed?.Invoke(this, new CreaturePlayedEventArgs
            {
                Creature = creature,
                Player = Opponent
            });
        }

        internal void SpawnCreature(string shortName, Guid creatureId, Guid playerId)
        {
            var player = GetPlayer(playerId);
            var creature = cardInstanceFactory.Create(shortName, creatureId, player) as CreatureCardInstance;
            player.Field.Add(creature);

            CreatureSpawned?.Invoke(this, new CreatureSpawnedEventArgs
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
                var instance = cardInstanceFactory.Create(card.ShortName, card.Id, Me);
                deck.Add(instance);
            }

            return deck;
        }
    }
}
