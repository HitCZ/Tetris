using Tetris.Logic;
using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for PauseMenu.xaml
    /// </summary>
    public partial class PauseMenuView
    {
        #region Properties

        public PauseMenuViewModel ViewModel
        {
            get => DataContext as PauseMenuViewModel;
            set => DataContext = value;
        }

        #endregion Properties

        #region Constructor

        public PauseMenuView(Game gameInstance)
        {
            InitializeComponent();

            var menuItems = new[] {
                resumeTextBlock,
                restartTextBlock,
                scoreTextBlock,
                exitTextBlock
            };

            ViewModel = new PauseMenuViewModel(this, gameInstance, menuItems);
        }

        #endregion Constructor
    }
}
