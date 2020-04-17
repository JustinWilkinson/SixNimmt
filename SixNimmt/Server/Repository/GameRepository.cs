using SixNimmt.Server.Extensions;
using SixNimmt.Shared;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace SixNimmt.Server.Repository
{
    public interface IGameRepository
    {
        void CreateGame(Game game);

        void SaveGame(Game game);

        Game GetGame(string id);

        IEnumerable<Game> ListGames();

        void DeleteGame(string id);
    }

    public class GameRepository : Repository, IGameRepository
    {
        public GameRepository() : base("CREATE TABLE IF NOT EXISTS Games (Id text, GameJson text)")
        {

        }

        public void CreateGame(Game game)
        {
            var command = new SQLiteCommand("INSERT INTO Games (Id, GameJson) VALUES(@Id, @Json)");
            command.AddParameter("@Id", game.Id);
            command.AddParameter("@Json", game.Serialize());
            Execute(command);
        }

        public void SaveGame(Game game)
        {
            var command = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id");
            command.AddParameter("@Id", game.Id);
            command.AddParameter("@Json", game.Serialize());
            Execute(command);
        }

        public Game GetGame(string id)
        {
            var command = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id");
            command.AddParameter("@Id", id);
            return Execute(command, DeserializeColumn<Game>("GameJson")).SingleOrDefault();
        }

        public IEnumerable<Game> ListGames() => Execute("SELECT * FROM Games", DeserializeColumn<Game>("GameJson"));

        public void DeleteGame(string id)
        {
            var command = new SQLiteCommand("DELETE FROM Games WHERE Id = @Id");
            command.AddParameter("@Id", id);
            Execute(command);
        }
    }
}