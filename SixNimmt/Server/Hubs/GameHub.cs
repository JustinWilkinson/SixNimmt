using Microsoft.AspNetCore.SignalR;
using SixNimmt.Shared;
using System.Threading.Tasks;

namespace Codenames.Server.Hubs
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string groupId);

        Task UpdateGameAsync(string gameId, string updatedGame);

        Task UpdatePlayerNameAsync(string gameId, string oldName, string newName);

        Task StartGameAsync(string gameId);

        Task PlayerSelectedCardAsync(string gameId, string playerName, Card card);

        Task BroadcastCoordinatesAsync(string gameId, Coordinates coordinates);
    }

    public class GameHub : Hub, IGameHub
    {
        public async Task AddToGroupAsync(string groupId) => await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);

        public async Task UpdatePlayerNameAsync(string gameId, string oldName, string newName) => await Clients.OthersInGroup(gameId).SendAsync("PlayerNameChanged", oldName, newName);

        public async Task StartGameAsync(string gameId) => await Clients.OthersInGroup(gameId).SendAsync("GameStarted");

        public async Task PlayerSelectedCardAsync(string gameId, string playerName, Card card) => await Clients.OthersInGroup(gameId).SendAsync("PlayerSelectedCard", playerName, card);

        public async Task BroadcastCoordinatesAsync(string gameId, Coordinates coordinates) => await Clients.OthersInGroup(gameId).SendAsync("CoordinatesReceived", coordinates);
    }
}