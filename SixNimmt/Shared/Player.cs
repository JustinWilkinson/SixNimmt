using System.Collections.Generic;

namespace SixNimmt.Shared
{
    public class Player
    {
        public string Name { get; set; }

        public int Points { get; set; }

        public List<Card> Hand { get; set; }

        public bool IsHost { get; set; }
    }
}