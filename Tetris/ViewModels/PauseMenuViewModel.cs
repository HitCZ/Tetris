using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Logic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public class PauseMenuViewModel
    {
        #region Properties

        public MenuManager MenuManager { get; set; }

        #endregion Properties

        #region Constructor

        public PauseMenuViewModel(PauseMenuView menuView, Game gameInstance, TextBlock[] menuItems)
        {
            var highlightColor = (SolidColorBrush)Application.Current.Resources["MenuItemHighlightBrush"];
            MenuManager = new MenuManager(menuView, menuItems, gameInstance, highlightColor);
        }

        #endregion Constructor
    }
}
