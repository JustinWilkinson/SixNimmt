using System;
using System.Collections.Generic;

namespace SixNimmt.Shared
{
    public class Deck
    {
        private readonly List<Card> _cards = new List<Card>();

        public Deck(int cardCount = 104)
        {
            for (var i = 0; i < cardCount; i++)
            {
                var points = 0;
                if (i % 5 == 0)
                {
                    points += 2;
                }
                if (i % 10 == 0)
                {
                    points += 1;
                }
                if (i % 11 == 0)
                {
                    points += 5;
                }
                points = points > 0 ? points : 1;
                _cards.Add(new Card { Value = i, Points = points });
            }
        }

        public void Shuffle()
        {
            Random rng = new Random();

            for (var i = 0; i < 5; i++)
            {
                var unshuffledCount = _cards.Count;
                while (unshuffledCount > 1)
                {
                    unshuffledCount--;
                    var newPosition = rng.Next(unshuffledCount + 1);
                    Card value = _cards[newPosition];
                    _cards[newPosition] = _cards[unshuffledCount];
                    _cards[unshuffledCount] = value;
                }
            }
        }

        public void Deal(IEnumerable<Player> players, int cardCount = 10)
        {
            for (var i = 0; i < cardCount; i++)
            {
                foreach (var player in players)
                {
                    player.Hand.Add(TopCard());
                }
            }
        }

        public Card TopCard()
        {
            var topCard = _cards[0];
            _cards.RemoveAt(0);
            return topCard;
        }
    }
}