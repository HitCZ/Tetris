using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainMenuView
    {
        #region Properties

        public MainMenuViewModel ViewModel
        {
            get => DataContext as MainMenuViewModel;
            set => DataContext = value;
        }

        #endregion Properties

        #region Constructor
        
        public MainMenuView()
        {
            InitializeComponent();
            var menuItems = new [] {
                StartTextBlock,
                ScoreTextBlock,
                ExitTextBlock
            };

            ViewModel = new MainMenuViewModel(this, menuItems);
        }

        #endregion Constructor
    }
}
