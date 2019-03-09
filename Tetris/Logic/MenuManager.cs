using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Tetris.Logic;
using Tetris.Views;

namespace Tetris
{
    public class MenuManager {
        // Aktualni okno
        private Window window;
        // Vsechny polozky menu v okne
        private TextBlock[] menuItems;
        // Vsechny mozne menu polozky ve hre
        private string[] possibleOptions =
            { "start", "tabulka skore", "konec", "restart", "pokracovat" };
        private Game gameInstance;
        // Barva zvyraznene polozky
        private Brush highlightColor;
        // Index zvyraznene polozky
        private int selectedItemIndex = 0;
        // Zvyraznena polozka
        private TextBlock selectedItem;

        public MenuManager(Window window, TextBlock[] menuItems,
            Game gameInstance, Brush color) {
            this.window = window;
            this.menuItems = menuItems;
            this.gameInstance = gameInstance;
            this.window.KeyDown += Window_KeyDown;
            this.highlightColor = color;

            this.FindSelectedItem();
            this.UpdateButtonColor();
            this.InitButtons();
        }

        public MenuManager(Window window, TextBlock[] menuItems, Brush color)
            : this(window, menuItems, null, color) {
        }

        /// <summary>
        /// Mys zvyrazni polozku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBlock_MouseEnter(object sender, MouseEventArgs e) {
            selectedItem = (TextBlock)sender;
            this.CalculateSelectedItem();
        
            selectedItem.Foreground = Brushes.White;
            UpdateButtonColor();
        }

        /// <summary>
        /// Mys vyjede mimo polozku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBlock_MouseLeave(object sender, MouseEventArgs e) {
            TextBlock item = (TextBlock)sender;
            item.Foreground = Brushes.Black;
        }

        /// <summary>
        /// Kliknuti mysi na polozku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBlock_MouseLeftButtonDown(object sender,
            MouseButtonEventArgs e) {
            AcceptItem();
        }

        /// <summary>
        /// Inicializace eventu pro jednotlive textBlocky
        /// </summary>
        private void InitButtons() {
            foreach (TextBlock t in menuItems) {
                t.MouseEnter += textBlock_MouseEnter;
                t.MouseLeave += textBlock_MouseLeave;
                t.PreviewMouseLeftButtonDown += textBlock_MouseLeftButtonDown;
            }
        }

        /// <summary>
        /// Zmeni hodnotu selectedItem podle itemu, nad kterym je kurzor
        /// </summary>
        private void CalculateSelectedItem() {
            for (int i = 0; i < menuItems.Length; i++) {
                if (menuItems[i].Text.Equals(selectedItem.Text)) {
                    selectedItemIndex = i;
                }
            }
        }

        /// <summary>
        /// Aktualizuje barvu polozek v menu => nezvyraznene polozky budou cerne
        /// </summary>
        private void UpdateButtonColor() {
            for (int i = 0; i < menuItems.Length; i++) {
                if (i == selectedItemIndex) {
                    menuItems[i].Foreground = this.highlightColor;
                }
                else {
                    menuItems[i].Foreground = Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Obsluha vstupu z klavesnice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
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
                        this.ResumeGame();
                        break;
                case Key.Enter:
                    AcceptItem();
                    break;
            }
            FindSelectedItem();
            UpdateButtonColor();
        }

        /// <summary>
        /// Najde zvoleny textBlock podle jeho indexu
        /// </summary>
        private void FindSelectedItem() {
            for (int i = 0; i < menuItems.Length; i++) {
                if (i == selectedItemIndex) {
                    selectedItem = menuItems[i];
                }
            }
        }

        /// <summary>
        /// Potvrdi zvolenou polozku
        /// </summary>
        private void AcceptItem() {
            Console.WriteLine("BUTTON SELECTED - " + selectedItem.Text);

            if (!ItemExists()) {
                throw new NotImplementedException(
                    "Menu item is not included among possible options.");
            }

            switch (selectedItem.Text.ToLower()) {
                case "start":
                    this.Start();
                    break;
                case "tabulka skore":
                    this.ShowScoreBoard();
                    break;
                case "restart":
                    this.RestartGame();
                    break;
                case "pokracovat":
                    this.ResumeGame();
                    break;
                case "konec":
                    this.ExitGame();
                    break;
            }
        }

        /// <summary>
        /// Kontrola, zda zvolena polozka existuje v seznamu possibleOptions
        /// </summary>
        /// <returns></returns>
        private bool ItemExists() {
            var text = selectedItem.Text.ToLower();

            foreach (string option in possibleOptions) {
                if (text.Equals(option))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Odstartuje hru
        /// </summary>
        private void Start() {
            Views.NamingView naming = new Views.NamingView();
            naming.Show();
            /*
            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Show();
            */
            this.window.Close();
        }

        /// <summary>
        /// Restartuje hru
        /// </summary>
        private void RestartGame() {
            this.gameInstance.CreateRecord();
            this.window.Close();
            gameInstance.StartNewGame();
        }

        /// <summary>
        /// Pokracovani v aktualni rozehrane hre
        /// </summary>
        private void ResumeGame() {
            this.window.Close();
            this.gameInstance.Resume();
        }

        /// <summary>
        /// Zobrazeni skore zebricku
        /// </summary>
        private void ShowScoreBoard() {
            ScoreBoardView scoreBoardView = new ScoreBoardView();
            scoreBoardView.Show();
        }

        /// <summary>
        /// Ukonceni hry
        /// </summary>
        private void ExitGame() {
            if (this.gameInstance != null && this.gameInstance.IsPaused)
                this.gameInstance.CreateRecord();
            App.Current.Shutdown();
        }
    }
}
