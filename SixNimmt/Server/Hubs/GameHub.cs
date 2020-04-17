using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Codenames.Server.Hubs
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string groupId);

        Task UpdateGameAsync(string gameId, string updatedGame);
    }

    public class GameHub : Hub, IGameHub
    {
        public async Task AddToGroupAsync(string groupId) => await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);
    }
}