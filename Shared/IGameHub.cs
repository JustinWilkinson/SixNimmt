using System.Threading.Tasks;

namespace SixNimmt.Shared
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string groupId);

        Task UpdateGameAsync(string gameId, string updatedGame);

        Task UpdatePlayerNameAsync(string gameId, string oldName, string newName);

        Task StartGameAsync(string gameId);

        Task PlayerSelectedCardAsync(string gameId, string playerName, Card card);

        Task BroadcastCoordinatesAsync(string gameId, Coordinates coordinates);

        Task NewGameAddedAsync();
    }
}