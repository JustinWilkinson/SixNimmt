using System;
using System.Collections.Generic;
using System.Linq;

namespace SixNimmt.Shared
{
    public class Game
    {
        public Guid Id { get; set; }

        public List<Player> Players { get; set; }

        public Card[,] CardRows { get; set; }

        public static Game NewGame(IEnumerable<Player> players, int rows = 4, int columns = 6)
        {
            var deck = new Deck();
            deck.Shuffle();
            deck.Deal(players);

            var game = new Game { Id = Guid.NewGuid(), Players = players.ToList() };

            game.CardRows = new Card[rows, columns];
            for (var i = 0; i < game.CardRows.Rank; i++)
            {
                game.CardRows[i, 0] = deck.TopCard();
            }

            return game;
        }
    }
}