using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SixNimmt.Server.Extensions;
using SixNimmt.Server.Repository;
using SixNimmt.Shared;
using System;
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

        public GameController(ILogger<GameController> logger, IGameRepository gameRepository)
        {
            _logger = logger;
            _gameRepository = gameRepository;
        }

        [HttpPut("New")]
        public Guid New(JsonElement playersJson)
        {
            var game = Game.NewGame(playersJson.Deserialize<IEnumerable<Player>>());
            _gameRepository.CreateGame(game);
            return game.Id;
        }

        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpPost("Save")]
        public void Save(JsonElement gameJson) => _gameRepository.SaveGame(gameJson.Deserialize<Game>());
    }
}