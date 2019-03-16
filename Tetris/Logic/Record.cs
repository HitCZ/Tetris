using System;

namespace Tetris.Logic
{
    public struct Record {
        #region Properties
        
        public string PlayerPlayerName { get; }
        public string Date { get; }
        public string Time { get; }

        public int SumOfClearedRows { get; }

        public int ReachedLevel { get; }

        public int Score { get; }

        #endregion Properties

        #region Constructor

        public Record(string playerName, int score, int sumOfClearedRows, int reachedLevel) {
            PlayerPlayerName = playerName;
            Date = DateTime.Today.ToString("dd.MM.yyyy");
            Time = DateTime.Now.ToString("HH:mm");
            Score = score;
            SumOfClearedRows = sumOfClearedRows;
            ReachedLevel = reachedLevel;
        }

        #endregion Constructor

    }
}