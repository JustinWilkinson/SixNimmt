using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Codenames.Server.Hubs
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string groupId);

        Task UpdateGameAsync(string gameId, string updatedGame);

        Task UpdatePlayerNameAsync(string gameId, string oldName, string newName);

        Task StartGameAsync(string gameId);
    }

    public class GameHub : Hub, IGameHub
    {
        public async Task AddToGroupAsync(string groupId) => await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);

        public async Task UpdatePlayerNameAsync(string gameId, string oldName, string newName) => await Clients.OthersInGroup(gameId).SendAsync("PlayerNameChanged", oldName, newName);

        public async Task StartGameAsync(string gameId) => await Clients.OthersInGroup(gameId).SendAsync("GameStarted");
    }
}