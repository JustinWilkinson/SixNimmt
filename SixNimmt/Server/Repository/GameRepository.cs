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

        Game GetGame(string id);

        IEnumerable<Game> ListGames();

        void Save(Game game);

        void ModifyGame(string gameId, Action<Game> action);

        T ModifyGame<T>(string gameId, Func<Game, T> function);
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

        public void Save(Game game)
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

        public void ModifyGame(string gameId, Action<Game> action)
        {
            ExecuteInTransaction(connection =>
            {
                var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
                selectCommand.AddParameter("@Id", gameId);
                using var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    action(game);
                    UpdateGame(connection, game);
                }
            });
        }

        public T ModifyGame<T>(string gameId, Func<Game, T> function)
        {
            T returnValue = default;

            ExecuteInTransaction(connection =>
            {
                var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
                selectCommand.AddParameter("@Id", gameId);
                using var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    returnValue = function(game);
                    UpdateGame(connection, game);
                }
            });

            return returnValue;
        }

        private void UpdateGame(SQLiteConnection connection, Game game)
        {
            var updateCommand = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id", connection);
            updateCommand.AddParameter("@Id", game.Id);
            updateCommand.AddParameter("@Json", game.Serialize());
            updateCommand.ExecuteNonQuery();
        }
    }
}