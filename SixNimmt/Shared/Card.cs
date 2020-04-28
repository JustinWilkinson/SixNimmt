using System;

namespace SixNimmt.Shared
{
    public class Card : IEquatable<Card>
    {
        public int Value { get; set; }

        public int Points { get; set; }

        public bool Equals(Card other) => Value == other.Value;
    }
}