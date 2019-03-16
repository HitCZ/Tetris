using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tetris.Annotations;
using Tetris.Logic;
using Tetris.Logic.Converters;

namespace Tetris.ViewModels
{
    public class ScoreBoardViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly DocumentToRecordsConverter documentConverter;
        private List<Record> scoreRecords;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the user score.
        /// </summary>
        public List<Record> ScoreRecords
        {
            get => scoreRecords;
            set
            {
                scoreRecords = value;
                OnPropertyChanged(nameof(ScoreRecords));
            }
        }

        #endregion Properties

        #region Constructor

        public ScoreBoardViewModel()
        {
            documentConverter = new DocumentToRecordsConverter();
            InitializeScore();
        }

        #endregion Constructor

        #region Private Methods

        private void InitializeScore()
        {
            var document = FileReader.GetInstance.Document;
            ScoreRecords = documentConverter.GetRecordsFromDocument(document);
        }

        #endregion Private Methods

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged members
    }
}
