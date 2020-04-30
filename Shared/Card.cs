using System;

namespace SixNimmt.Shared
{
    public class Card : IEquatable<Card>, IComparable<Card>
    {
        public int Value { get; set; }

        public int Points { get; set; }

        public static bool operator >(Card card1, Card card2) => card1.Value > card2.Value;

        public static bool operator <(Card card1, Card card2) => card1.Value < card2.Value;

        public bool Equals(Card other) => Value == other.Value;

        public int CompareTo(Card other) => Value == other.Value ? 0 : Value > other.Value ? 1 : -1;
    }
}