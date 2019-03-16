using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Tetris.Properties;
using Tetris.Views;

namespace Tetris.Logic
{
    public class MenuManager
    {
        #region Fields

        private readonly string[] possibleOptions =
        {
            Strings.MENU_START, Strings.MENU_SCOREBOARD, Strings.MENU_QUIT, Strings.MENU_RESTART, Strings.MENU_RESUME
        };
        private readonly Window window;
        private readonly TextBlock[] menuItems;
        private readonly Game gameInstance;
        private readonly Brush highlightColor;
        private int selectedItemIndex;
        private TextBlock selectedItem;

        #endregion Fields

        #region Constructor

        public MenuManager(Window window, TextBlock[] menuItems,
            Game gameInstance, Brush color)
        {
            this.window = window;
            this.menuItems = menuItems;
            this.gameInstance = gameInstance;
            this.window.KeyDown += Window_KeyDown;
            highlightColor = color;

            selectedItem = FindSelectedItemByIndex(selectedItemIndex);
            InitializeItemColors();
            InitButtons();
        }

        public MenuManager(Window window, TextBlock[] menuItems, Brush color)
            : this(window, menuItems, null, color)
        {
        }

        #endregion Constructor

        #region Event Handlers

        private void textBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is TextBlock txtBlock))
                return;

            selectedItem = txtBlock;
            selectedItemIndex = GetIndexForItem(selectedItem);

            selectedItem.Foreground = (SolidColorBrush)Application.Current.Resources["PauseMenuItemHighlightBrush"];
            InitializeItemColors();
        }

        private void textBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is TextBlock txtBlock))
                return;

            var item = txtBlock;
            item.Foreground = (SolidColorBrush)Application.Current.Resources["MenuItemDefaultColor"];
        }

        private void textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => AcceptItem(selectedItem);

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (selectedItemIndex == 0)
                        selectedItemIndex = menuItems.Length - 1;
                    else
                        selectedItemIndex--;
                    break;
                case Key.Down:
                    if (selectedItemIndex == menuItems.Length - 1)
                        selectedItemIndex = 0;
                    else
                        selectedItemIndex++;
                    break;
                case Key.Escape:
                    if (gameInstance != null)
                        ResumeGame();
                    break;
                case Key.Enter:
                    AcceptItem(selectedItem);
                    break;
            }

            selectedItem = FindSelectedItemByIndex(selectedItemIndex);
            InitializeItemColors();
        }

        #endregion Event Handlers


        #region Private Methods

        private void InitButtons()
        {
            foreach (var t in menuItems)
            {
                t.MouseEnter += textBlock_MouseEnter;
                t.MouseLeave += textBlock_MouseLeave;
                t.PreviewMouseLeftButtonDown += textBlock_MouseLeftButtonDown;
            }
        }

        private int GetIndexForItem(TextBlock item)
        {
            for (var i = 0; i < menuItems.Length; i++)
            {
                if (menuItems[i].Text.Equals(item.Text, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            throw new ArgumentException($"{nameof(item)} was not found among the menu items.");
        }

        private void InitializeItemColors()
        {
            for (var i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].Foreground = i == selectedItemIndex ? highlightColor : Brushes.Black;
            }
        }

        private TextBlock FindSelectedItemByIndex(int index)
        {
            for (var i = 0; i < menuItems.Length; i++)
            {
                if (i == index)
                    return menuItems[i];
            }

            throw new ArgumentException($"Item with the index \"{index}\" does not exist.");
        }

        private void AcceptItem(TextBlock item)
        {
            if (!ItemExists(item))
                throw new ArgumentException($"Menu item {item} is not included among possible options.");

            if (selectedItem.Text.ToLower() == Strings.MENU_START)
                Start();
            else if (selectedItem.Text.ToLower() == Strings.MENU_SCOREBOARD)
                ShowScoreBoard();
            else if (selectedItem.Text.ToLower() == Strings.MENU_RESTART)
                RestartGame();
            else if (selectedItem.Text.ToLower() == Strings.MENU_RESUME)
                ResumeGame();
            else if (selectedItem.Text.ToLower() == Strings.MENU_QUIT)
                ExitGame();
        }

        private bool ItemExists(TextBlock item)
        {
            var text = item.Text.ToLower();

            return possibleOptions.Any(x => x.Equals(text));
        }

        private void Start()
        {
            var naming = new NamingView();
            naming.Show();
            window.Close();
        }

        private void RestartGame()
        {
            gameInstance.CreateRecord();
            window.Close();
            gameInstance.StartNewGame();
        }

        private void ResumeGame()
        {
            window.Close();
            gameInstance.Resume();
        }

        private void ShowScoreBoard()
        {
            var scoreBoardView = new ScoreBoardView();
            scoreBoardView.Show();
        }

        private void ExitGame()
        {
            if (gameInstance != null && gameInstance.IsPaused)
                gameInstance.CreateRecord();
            Application.Current.Shutdown();
        }

        #endregion Private Methods
    }
}
