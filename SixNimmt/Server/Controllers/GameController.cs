using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SixNimmt.Server.Extensions;
using SixNimmt.Server.Repository;
using SixNimmt.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public void New(JsonElement gameId) => _gameRepository.CreateGame(new Game { Id = new Guid(gameId.GetString()), Players = new List<Player> { new Player { Name = "Host", IsHost = true, Hand = new List<Card>() } } });

        [HttpPost("Join")]
        public Player Join(JsonElement json)
        {
            var gameId = json.GetString();
            lock (_locks.GetOrAdd(gameId, new object()))
            {
                var game = _gameRepository.GetGame(gameId);
                var player = new Player { Name = $"Player {game.Players.Count}", Hand = new List<Card>() };
                game.Players.Add(player);
                _gameRepository.SaveGame(game);
                return player;
            }
        }

        [HttpPost("UpdatePlayer")]
        public void UpdatePlayer(JsonElement json)
        {
            _gameRepository.UpdatePlayerName(json.GetStringProperty("GameId"), json.GetStringProperty("OldName"), json.GetStringProperty("NewName"));
        }

        [HttpPost("SelectCard")]
        public void SelectCard(JsonElement json)
        {
            _gameRepository.SelectCard(json.GetStringProperty("GameId"), json.GetStringProperty("PlayerName"), json.DeserializeStringProperty<Card>("Card"));
        }

        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpPost("Save")]
        public void Save(JsonElement gameJson) => _gameRepository.SaveGame(gameJson.Deserialize<Game>());

        [HttpPost("StartGame")]
        public void StartGame(JsonElement gameJson)
        {
            var game = gameJson.Deserialize<Game>();
            game.StartGame();
            _gameRepository.SaveGame(game);
        }
    }
}