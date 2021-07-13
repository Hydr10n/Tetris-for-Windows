using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Tetris.Game.BasicDataTypes;

namespace Tetris.Data
{
    public class ScoreboardData
    {
        public struct Record
        {
            public int Score { get; set; }
            public int LineCount { get; set; }
            public int Level { get; set; }
            public string Difficulty { get; set; }
            public string Time { get; set; }
            public string DateCreated { get; set; }
        }

        public IEnumerable<Record> Records { get; private set; }

        public ScoreboardData(Difficulty difficulty)
        {
            try
            {
                Records = from element in MyAppData.Load()
                          where element.Name == nameof(Record) && element.Attribute(nameof(Record.Difficulty))?.Value == difficulty.ToString()
                          select new Record()
                          {
                              Score = int.Parse(element.Element(nameof(Record.Score))?.Value),
                              LineCount = int.Parse(element.Element(nameof(Record.LineCount))?.Value),
                              Level = int.Parse(element.Element(nameof(Record.Level))?.Value),
                              Time = element.Element(nameof(Record.Time))?.Value,
                              DateCreated = element.Element(nameof(Record.DateCreated))?.Value
                          };
                var sortDescriptions = CollectionViewSource.GetDefaultView(Records).SortDescriptions;
                sortDescriptions.Add(new SortDescription(nameof(Record.Score), ListSortDirection.Descending));
                sortDescriptions.Add(new SortDescription(nameof(Record.LineCount), ListSortDirection.Descending));
                sortDescriptions.Add(new SortDescription(nameof(Record.Level), ListSortDirection.Descending));
                sortDescriptions.Add(new SortDescription(nameof(Record.Time), ListSortDirection.Ascending));
                sortDescriptions.Add(new SortDescription(nameof(Record.DateCreated), ListSortDirection.Descending));
            }
            catch { }
        }
    }
}
