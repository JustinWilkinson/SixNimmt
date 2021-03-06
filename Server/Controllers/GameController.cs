﻿using Microsoft.AspNetCore.Mvc;
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
        public void New(JsonElement json)
        {
            _gameRepository.CreateGame(new Game
            {
                Id = new Guid(json.GetStringProperty("GameId")),
                Name = json.GetStringProperty("GameName") ?? "Unnamed Game",
                Players = new List<Player>(),
                CreatedAtUtc = DateTime.UtcNow,
                VariableCardCount = json.GetBooleanProperty("VariableCardCount")
            }, json.GetBooleanProperty("PrivateGame"));
        }

        [HttpPost("Join")]
        public Player Join(JsonElement gameIdJson)
        {
            return _gameRepository.ModifyGame(gameIdJson.GetString(), game =>
            {
                var playerCount = game.Players.Count;
                var isHost = playerCount == 0;

                if (playerCount == 10 && !game.VariableCardCount)
                {
                    return null;
                }
                else
                {
                    var player = new Player { Name = isHost ? "Host" : $"Guest {playerCount}", Hand = new List<Card>(), IsHost = isHost };
                    game.Players.Add(player);
                    return player;
                }
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
        public string List() => _gameRepository.ListGames(false).Serialize();

        [HttpPost("Start")]
        public void Start(JsonElement gameIdJson) => _gameRepository.ModifyGame(gameIdJson.GetString(), game => game.StartGame());

        [HttpPost("RemovePlayer")]
        public string RemovePlayer(JsonElement json)
        {
            return _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game =>
            {
                game.Players.RemoveAt(game.Players.FindIndex(p => p.Name == json.GetStringProperty("KickedPlayerName")));
                return game.Serialize();
            });
        }
    }
}