using System;
using System.Collections.Generic;

namespace SixNimmt.Shared
{
    public class Game
    {
        public Guid Id { get; set; }

        public List<Player> Players { get; set; }

        public Card[,] CardRows { get; set; }

        public bool ShowHand { get; set; }

        public int Round { get; set; }

        public bool RoundEnded { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Ended { get; set; }

        public void StartRound(int rows = 4, int columns = 6)
        {
            var deck = new Deck();
            deck.Shuffle();
            deck.Deal(Players);
            Players.ForEach(x => x.Hand.Sort((c1, c2) => c1.Value.CompareTo(c2.Value)));

            CardRows = new Card[rows, columns];
            for (var i = 0; i < CardRows.GetLength(0); i++)
            {
                CardRows[i, 0] = deck.TopCard();
            }

            Round++;
            RoundEnded = false;
            ShowHand = true;
        }

        public void StartGame(int rows = 4, int columns = 6)
        {
            StartRound(rows, columns);
            Started = DateTime.UtcNow;
        }
    }
}