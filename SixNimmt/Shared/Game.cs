using System;
using System.Collections.Generic;

namespace SixNimmt.Shared
{
    public class Game
    {
        public Guid Id { get; set; }

        public bool Started { get; set; }

        public List<Player> Players { get; set; }

        public Card[,] CardRows { get; set; }

        public void StartGame(int rows = 4, int columns = 6)
        {
            var deck = new Deck();
            deck.Shuffle();
            deck.Deal(Players);

            CardRows = new Card[rows, columns];
            for (var i = 0; i < CardRows.GetLength(0); i++)
            {
                CardRows[i, 0] = deck.TopCard();
            }

            Started = true;
        }
    }
}