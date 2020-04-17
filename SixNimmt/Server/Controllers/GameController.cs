using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SixNimmt.Server.Extensions;
using SixNimmt.Server.Repository;
using SixNimmt.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SixNimmt.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameRepository _gameRepository;

        private static readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();

        public GameController(ILogger<GameController> logger, IGameRepository gameRepository)
        {
            _logger = logger;
            _gameRepository = gameRepository;
        }

        [HttpPut("New")]
        public void New(JsonElement gameId) => _gameRepository.CreateGame(new Game { Id = new Guid(gameId.GetString()), Players = new List<Player> { new Player { Name = "Host", IsHost = true } } });

        [HttpPost("Join")]
        public Player Join(JsonElement json)
        {
            var gameId = json.GetString();
            lock (_locks.GetOrAdd(gameId, new object()))
            {
                var game = _gameRepository.GetGame(gameId);
                var player = new Player { Name = $"Player {game.Players.Count}" };
                game.Players.Add(player);
                _gameRepository.SaveGame(game);
                return player;
            }
        }

        [HttpPost("UpdatePlayer")]
        public void UpdatePlayer(JsonElement json)
        {
            var gameId = json.GetProperty("GameId").GetString();
            lock (_locks.GetOrAdd(gameId, new object()))
            {
                var game = _gameRepository.GetGame(gameId);
                game.Players.Single(p => p.Name == json.GetProperty("OldName").GetString()).Name = json.GetProperty("NewName").GetString();
                _gameRepository.SaveGame(game);
            }
        }

        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpPost("Save")]
        public void Save(JsonElement gameJson) => _gameRepository.SaveGame(gameJson.Deserialize<Game>());

        [HttpDelete("Delete")]
        public void Delete(string gameId)
        {
            _locks.TryRemove(gameId, out _);
            _gameRepository.DeleteGame(gameId);
        }
    }
}