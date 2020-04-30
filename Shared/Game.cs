﻿using System;
using System.Collections.Generic;

namespace SixNimmt.Shared
{
    public class Game
    {
        public Guid Id { get; set; }

        public List<Player> Players { get; set; }

        public Board Board { get; set; }

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
            Players.ForEach(x => x.Hand.Sort());
            Board = new Board(deck, rows, columns);

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