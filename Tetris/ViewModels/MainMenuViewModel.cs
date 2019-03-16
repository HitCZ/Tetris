using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Logic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public class MainMenuViewModel
    {
        #region Properties
        
        public MenuManager MenuManager { get; set; }

        #endregion Properties

        #region Constructor
        
        public MainMenuViewModel(MainMenuView mainMenuView, TextBlock[] menuItems)
        {
            var highlightColor = (SolidColorBrush)Application.Current.Resources["MainMenuItemHighlightBrush"];
            MenuManager = new MenuManager(mainMenuView, menuItems, highlightColor);
        }

        #endregion Constructor
    }
}
