using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using CardGame.Client.Instances.Cards;
using CardGame.Client.Instances.Players;
using CardGame.Client.Models.PlayerActions.Cards.Creatures;
using CardGame.Client.Models.PlayerActions.Game;
using CardGame.Client.Networking;
using CardGame.Shared.Enums;
using CardGame.Shared.Models.Players;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SampleGame.Cards.Creatures;
using SampleGame.Client.Web.Models;
using SampleGame.Client.Web.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleGame.Client.Web.Pages
{
    public partial class Index
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Inject] private IToastService ToastService { get; set; }

        private SingleGameClient client;
        private bool GameStarted => client != null && client.Game != null && client.Game.Status != GameStatus.Created;
        private readonly ConnectionInfo connectionInfo = new();

        private OpponentInstance Opponent => client.Game.Opponent;
        private MeInstance Me => client.Game.Me;

        private void Connect()
        {
            client = new SingleGameClient(typeof(BasicSoldier).Assembly, new Player
            {
                Name = connectionInfo.Name,
                Deck = new Dictionary<string, int>
                {
                    { "BasicSoldier", 10 },
                    { "Attacker", 5 },
                    { "Booster", 5 },
                    { "Defender", 3 },
                    { "Gunner", 5 },
                    { "Medic", 3 },
                    { "Quickshot", 5 }
                }
            });

            BindClientEvents();
            BindGameEvents();
            client.InnerClient.Connect(connectionInfo.Host, connectionInfo.Port, connectionInfo.Key);
        }

        private async Task PlayCard()
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(ChooseCard.Cards), client.Game.Me.Hand);

            var modal = Modal.Show<ChooseCard>("Choose card", parameters);
            var result = await modal.Result;

            if (!result.Cancelled)
            {
                client.PlayerActionHandler.PerformAction(new PlayCardAction
                {
                    Card = (CardInstance)result.Data
                });
            }
        }

        private async Task AttackPlayer()
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(ChooseCard.Cards), client.Game.Me.Field.Where(c => c.CanAttack));

            var modal = Modal.Show<ChooseCard>("Choose card", parameters);
            var result = await modal.Result;

            if (!result.Cancelled)
            {
                client.PlayerActionHandler.PerformAction(new AttackPlayerAction
                {
                    Attacker = (CreatureCardInstance)result.Data
                });
            }
        }

        private async Task AttackCreature()
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(ChooseCard.Cards), client.Game.Me.Field.Where(c => c.CanAttack));

            var modal = Modal.Show<ChooseCard>("Choose card", parameters);
            var result = await modal.Result;

            if (!result.Cancelled)
            {
                var attacker = (CreatureCardInstance)result.Data;

                parameters = new ModalParameters();
                parameters.Add(nameof(ChooseCard.Cards), client.Game.Opponent.Field);

                modal = Modal.Show<ChooseCard>("Choose card", parameters);
                result = await modal.Result;

                if (!result.Cancelled)
                {
                    client.PlayerActionHandler.PerformAction(new AttackCreatureAction
                    {
                        Attacker = attacker,
                        Target = (CreatureCardInstance)result.Data
                    });
                }
            }
        }

        private void EndTurn() => client.PlayerActionHandler.PerformAction(new EndTurnAction());
        private void Surrender() => client.PlayerActionHandler.PerformAction(new SurrenderAction());

        private void BindClientEvents()
        {
            client.GameJoined += (sender, playerId)
                => LogSuccess($"Game joined. My id is {playerId}");

            client.InnerClient.MessageReceived += (sender, message)
                => LogJs($"Server message: {message}");

            client.InnerClient.MessageSent += (sender, message)
                => LogJs($"Client message: {message}");

            client.ServerMessageHandler.InvalidMessageReceived += (sender, message)
                => LogJs($"Invalid message: {message}");

            client.ServerMessageHandler.Exception += (sender, ex)
                => LogException(ex);

            client.GameEventHandler.Error += (sender, error) 
                => LogError(error);
        }

        private void BindGameEvents()
        {
            client.Game.GameStarted += (sender, e) =>
            {
                LogInfo($"Game started. Your opponent is {client.Game.Opponent.Name}. {e.CurrentPlayer.Name} goes first");
                InvokeAsync(StateHasChanged);
            };

            client.Game.GameEnded += (sender, e) =>
            {
                LogInfo($"Game ended, {client.Game.Winner.Name} is the winner");
                InvokeAsync(StateHasChanged);
            };

            client.Game.NewTurn += (sender, e) =>
            {
                LogInfo($"Turn ended. {e.CurrentPlayer.Name} plays the next turn");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CardsDrawn += (sender, e) =>
            {
                LogInfo($"You drew {e.NewCards.Count} cards");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CardsDrawnOpponent += (sender, e) =>
            {
                LogInfo($"The opponent drew {e.Amount} cards");
                InvokeAsync(StateHasChanged);
            };

            client.Game.PlayerAttacked += (sender, e) =>
            {
                LogInfo($"{e.Attacker.ShortName} attacked {e.Player.Name} and dealt {e.Damage} damage");
                InvokeAsync(StateHasChanged);
            };

            client.Game.PlayerDamaged += (sender, e) =>
            {
                LogInfo($"{e.Player.Name} received {e.Damage} damage");
                InvokeAsync(StateHasChanged);
            };

            client.Game.PlayerHealthRestored += (sender, e) =>
            {
                LogInfo($"{e.Player.Name}'s health restored by {e.Amount} (health: {e.Player.CurrentHealth})");
                InvokeAsync(StateHasChanged);
            };

            client.Game.PlayerManaRestored += (sender, e) =>
            {
                LogInfo($"{e.Player.Name}'s mana restored by {e.Amount} (mana: {e.Player.CurrentMana} / {e.Player.MaximumMana})");
                InvokeAsync(StateHasChanged);
            };

            client.Game.PlayerManaSpent += (sender, e) =>
            {
                LogInfo($"{e.Player.Name} spent {e.Amount} mana (mana: {e.Player.CurrentMana} / {e.Player.MaximumMana})");
                InvokeAsync(StateHasChanged);
            };

            client.Game.PlayerMaxManaIncreased += (sender, e) =>
            {
                LogInfo($"{e.Player.Name}'s maximum mana increased by {e.Increment} (mana: {e.Player.CurrentMana} / {e.Player.MaximumMana})");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CreatureAttacked += (sender, e) =>
            {
                LogInfo($"{e.Attacker.ShortName} attacked {e.Defender.ShortName}, dealt {e.Damage} damage and received {e.RecoilDamage} recoil damage");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CreatureAttackChanged += (sender, e) =>
            {
                LogInfo($"{e.Creature.ShortName}'s attack changed to {e.NewValue}");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CreatureDamaged += (sender, e) =>
            {
                LogInfo($"{e.Target.ShortName} took {e.Damage} damage");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CreatureDestroyed += (sender, e) =>
            {
                LogInfo($"{e.Target.ShortName} was destroyed");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CreatureHealthIncreased += (sender, e) =>
            {
                LogInfo($"{e.Creature.ShortName}'s health was increased by {e.Amount}");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CreaturePlayed += (sender, e) =>
            {
                LogInfo($"{e.Player.Name} played {e.Creature.ShortName} from their hand");
                InvokeAsync(StateHasChanged);
            };

            client.Game.CreatureSpawned += (sender, e) =>
            {
                LogInfo($"{e.Creature.ShortName} spawned on {e.Creature.Owner.Name}'s field");
                InvokeAsync(StateHasChanged);
            };
        }

        private void LogInfo(string message, string title = "")
        {
            ToastService.ShowInfo(message, title);
            LogJs(message);
        }

        private void LogSuccess(string message, string title = "")
        {
            ToastService.ShowSuccess(message, title);
            LogJs(message);
        }

        private void LogWarning(string message, string title = "")
        {
            ToastService.ShowWarning(message, title);
            LogJs(message);
        }

        private void LogError(string message, string title = "")
        {
            ToastService.ShowError(message, title);
            LogJs(message);
        }

        private void LogException(Exception ex)
        {
            ToastService.ShowError(ex.Message, "Exception");
            LogJs(ex.ToString());
        }

        private void LogJs(string message)
            => _ = js.InvokeVoidAsync("console.log", message);
    }
}
