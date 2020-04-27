using Microsoft.Extensions.Logging;
using SixNimmt.Server.Extensions;
using SixNimmt.Shared;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace SixNimmt.Server.Repository
{
    public interface IGameRepository
    {
        void CreateGame(Game game);

        void SaveGame(Game game);

        void UpdatePlayerName(string gameId, string oldName, string newName);

        Game GetGame(string id);

        IEnumerable<Game> ListGames();

        void DeleteGame(string id);
    }

    public class GameRepository : Repository, IGameRepository
    {
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(ILogger<GameRepository> logger) : base("CREATE TABLE IF NOT EXISTS Games (Id text, GameJson text)")
        {
            _logger = logger;
        }

        public void CreateGame(Game game)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Games (Id, GameJson) VALUES(@Id, @Json)");
                command.AddParameter("@Id", game.Id);
                command.AddParameter("@Json", game.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating a new game.");
                throw;
            }
        }

        public void SaveGame(Game game)
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id");
                command.AddParameter("@Id", game.Id);
                command.AddParameter("@Json", game.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred saving game '{game.Id}'.");
                throw;
            }
        }

        public void UpdatePlayerName(string gameId, string oldName, string newName)
        {
            ExecuteInTransaction((connection) =>
            {
                var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
                selectCommand.AddParameter("@Id", gameId);
                using var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    game.Players.Single(p => p.Name == oldName).Name = newName;
                    var updateCommand = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id", connection);
                    updateCommand.AddParameter("@Id", game.Id);
                    updateCommand.AddParameter("@Json", game.Serialize());
                    updateCommand.ExecuteNonQuery();
                }
            });
        }

        public Game GetGame(string id)
        {
            try
            {
                var command = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id");
                command.AddParameter("@Id", id);
                return Execute(command, DeserializeColumn<Game>("GameJson")).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving game '{id}'.");
                throw;
            }
        }

        public IEnumerable<Game> ListGames()
        {
            try
            {
                return Execute("SELECT * FROM Games", DeserializeColumn<Game>("GameJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred listing games.");
                throw;
            }
        }

        public void DeleteGame(string id)
        {
            try
            {
                var command = new SQLiteCommand("DELETE FROM Games WHERE Id = @Id");
                command.AddParameter("@Id", id);
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred deleting game {id}.");
                throw;
            }
        }
    }
}