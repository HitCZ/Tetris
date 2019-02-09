using System;

namespace Tetris {
    public struct Record {
        private readonly string playerName;
        private readonly string date;
        private readonly string time;
        private readonly int score;
        private readonly int sumOfClearedRows;
        private readonly int reachedLevel;

        public Record(string name, int score, int sumOfClearedRows, 
            int reachedLevel) {
            this.playerName = name;
            this.date = DateTime.Today.ToString("dd.MM.yyyy");
            this.time = DateTime.Now.ToString("HH:mm");
            this.score = score;
            this.sumOfClearedRows = sumOfClearedRows;
            this.reachedLevel = reachedLevel;
        }

        public string PlayerName
        {
            get
            {
                return this.playerName;
            }
        }
        public string Date
        {
            get { return this.date; }
        }

        public string Time
        {
            get { return this.time; }
        }

        public int SumOfCLearedRows
        {
            get { return this.sumOfClearedRows; }
        }

        public int ReachedLevel
        {
            get { return this.reachedLevel; }
        }

        public int Score
        {
            get { return this.score; }
        }
    }
}