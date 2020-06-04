using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SixNimmt.Server.Repository;
using SixNimmt.Shared;
using SixNimmt.Shared.Extensions;
using System;
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

        public GameController(ILogger<GameController> logger, IGameRepository gameRepository)
        {
            _logger = logger;
            _gameRepository = gameRepository;
        }

        [HttpPut("New")]
        public void New(JsonElement gameId)
        {
            _gameRepository.CreateGame(new Game
            {
                Id = new Guid(gameId.GetString()),
                Players = new List<Player> { new Player { Name = "Host", IsHost = true, Hand = new List<Card>() } }
            });
        }

        [HttpPost("Join")]
        public Player Join(JsonElement gameIdJson)
        {
            return _gameRepository.ModifyGame(gameIdJson.GetString(), game =>
            {
                var player = new Player { Name = $"Player {game.Players.Count}", Hand = new List<Card>() };
                game.Players.Add(player);
                return player;
            });
        }

        [HttpPost("UpdatePlayer")]
        public void UpdatePlayer(JsonElement json)
        {
            _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game => game.Players.Single(p => p.Name == json.GetStringProperty("OldName")).Name = json.GetStringProperty("NewName"));
        }

        [HttpPost("SelectCard")]
        public void SelectCard(JsonElement json)
        {
            var newSelectedCard = json.DeserializeStringProperty<Card>("Card");
            _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game =>
            {
                if (game.ShowHand)
                {
                    var player = game.Players.Single(p => p.Name == json.GetStringProperty("PlayerName"));
                    var oldSelectedCard = player.SelectedCard;
                    if (oldSelectedCard != null)
                    {
                        player.Hand.Add(oldSelectedCard);
                        player.Hand.Sort();
                    }
                    if (newSelectedCard != null)
                    {
                        player.Hand.RemoveAt(player.Hand.FindIndex(c => c.Value == newSelectedCard.Value));
                    }
                    player.SelectedCard = newSelectedCard;
                    game.ShowHand = game.Players.Any(p => p.SelectedCard == null);
                }
            });
        }

        [HttpPost("Save")]
        public void Save(JsonElement json) => _gameRepository.Save(json.Deserialize<Game>());

        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpPost("StartGame")]
        public void StartGame(JsonElement gameIdJson) => _gameRepository.ModifyGame(gameIdJson.GetString(), game => game.StartGame());
    }
}