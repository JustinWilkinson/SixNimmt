using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SixNimmt.Shared
{
    public class Board
    {
        [JsonProperty]
        private Card[,] CardRows { get; set; }

        public Board(Deck deck, int rows, int columns)
        {
            CardRows = new Card[rows, columns];
            for (var i = 0; i < RowCount; i++)
            {
                CardRows[i, 0] = deck.TopCard();
            }
        }

        [JsonIgnore]
        public int RowCount => CardRows.GetLength(0);

        [JsonIgnore]
        public int ColumnCount => CardRows.GetLength(1);

        public Card this[int row, int column] { get => CardRows[row, column]; set => CardRows[row, column] = value; }

        public IEnumerable<Card> this[int index, Index rowOrColumn = Index.Row]
        {
            get
            {
                if (rowOrColumn == Index.Row)
                {
                    for (var i = 0; i < ColumnCount; i++)
                    {
                        yield return CardRows[index, i];
                    }
                }
                else
                {
                    for (var i = 0; i < RowCount; i++)
                    {
                        yield return CardRows[i, index];
                    }
                }
            }
        }

        public IEnumerable<IEnumerable<Card>> GetRows()
        {
            for (int i = 0; i < RowCount; i++)
            {
                yield return this[i];
            }
        }

        public IEnumerable<IEnumerable<Card>> GetColumns()
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                yield return this[i, Index.Column];
            }
        }

        public Card GetLastCardInRow(int row) => this[row].Last(c => c != null);

        public int GetLastCardIndexInRow(int row) => this[row].Count(c => c != null) - 1;

        public Card GetCardWithMinimumDifference(Card card)
        {
            return card != null ? 
                GetRows().Select((row, index) => GetLastCardInRow(index)).Where(lastCardInRow => card > lastCardInRow).OrderByDescending(c => c.Value).FirstOrDefault() :
                null;    
        }
    }

    public enum Index
    {
        Row,
        Column
    }
}