using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cg2016Excer1
{

    public class PlayerData
    {
        public Guid playerid;
        public string FirstName;
        public string SecondName;
        public string GamerTag;
        public int topscore;

        public string GamerTagScore { get { return GamerTag + " 8==D "+ topscore.ToString(); } }

        public PlayerData()
        {
        }
        public static PlayerData FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            PlayerData dailyValues = new PlayerData();
            dailyValues.playerid = Guid.NewGuid();
            dailyValues.FirstName = values[0];
            dailyValues.SecondName = values[1];
            dailyValues.GamerTag = values[2];
            dailyValues.topscore = Int32.Parse(values[3]);

            return dailyValues;
        }
    }
    // Implement IDisposable to allow using 
    class TestDbContext :IDisposable
    {
        private bool disposed = false;
        public List<PlayerData> ScoreBoard = new List<PlayerData>();

        public TestDbContext()
        {
              ScoreBoard = File.ReadAllLines(@"Content\random Names with scores.csv")
                                           //.Skip(1) // Only needed if the first row contains the Field names
                                           .Select(v => PlayerData.FromCsv(v))
                                           .OrderByDescending(s => s.topscore)
                                           .ToList();

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

            if (!disposed)
            {
                if (disposing)
                {
                    // Manual release of managed resources.
                }
                // Release unmanaged resources.
                disposed = true;
            }
        }

        public List<PlayerData> getTop(int count)
        {
            return ScoreBoard.Take(count).ToList();
        }

    }
}
