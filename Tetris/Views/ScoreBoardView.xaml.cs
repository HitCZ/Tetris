using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for ScoreBoard.xaml
    /// </summary>
    public partial class ScoreBoardView {

        #region Properties
        
        public ScoreBoardViewModel ViewModel
        {
            get => DataContext as ScoreBoardViewModel;
            set => DataContext = value;
        }

        #endregion Properties

        public ScoreBoardView() {                
            InitializeComponent();
            ViewModel = new ScoreBoardViewModel();
        }
    }
}
