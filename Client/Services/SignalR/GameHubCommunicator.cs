using SixNimmt.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace SixNimmt.Client.Services.SignalR
{
    public class GameHubCommunicator : HubCommunicator, IGameHub
    {
        public GameHubCommunicator(NavigationManager navigationManager) : base("/GameHub", navigationManager)
        {

        }

        public async Task AddToGroupAsync(string groupId) => await _hubConnection.InvokeAsync("AddToGroupAsync", groupId);

        public async Task RemoveFromGroupAsync(string groupId) => await _hubConnection.InvokeAsync("RemoveFromGroupAsync", groupId);

        public async Task PlayerSelectedCardAsync(string gameId, string playerName, Card card) => await _hubConnection.InvokeAsync("PlayerSelectedCardAsync", gameId, playerName, card);

        public async Task BroadcastCoordinatesAsync(string gameId, Coordinates coordinates) => await _hubConnection.InvokeAsync("BroadcastCoordinatesAsync", gameId, coordinates);

        public async Task NewGameAddedAsync() => await _hubConnection.InvokeAsync("NewGameAddedAsync");

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await _hubConnection.InvokeAsync("UpdateGameAsync", gameId, updatedGame);

        public async Task UpdatePlayerNameAsync(string gameId, string oldName, string newName) => await _hubConnection.InvokeAsync("UpdatePlayerNameAsync", gameId, oldName, newName);

        public async Task StartGameAsync(string gameId) => await _hubConnection.InvokeAsync("StartGameAsync");
    }
}