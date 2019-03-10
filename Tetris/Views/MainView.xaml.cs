using System;
using System.Windows;
using System.Windows.Input;
using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        #region Properties

        public MainViewModel ViewModel { get; set; }

        #endregion Properties

        #region Constructor
        
        public MainView(string playerName)
        {
            InitializeComponent();
            ViewModel = new MainViewModel(this, GameGrid.RowDefinitions.Count, GameGrid.ColumnDefinitions.Count, playerName);
            DataContext = ViewModel;

            ViewModel.ClearNextShapeGridAction += () => GridNextShape.Children.Clear();
            ViewModel.AddBlockToNextShapeGridAction += (block) => GridNextShape.Children.Add(block);
            ViewModel.ClearGameGridAction += () => GameGrid.Children.Clear();
            ViewModel.AddBlockToGameGridAction += (block) => GameGrid.Children.Add(block);
            ViewModel.RemoveBlockFromGameGridAction += (block) => GameGrid.Children.Remove(block);

            Loaded += MainView_Loaded;
            KeyDown += MainView_KeyDown;
        }

        #endregion Constructor

        #region Event Handlers

        private void MainView_Loaded(object sender, RoutedEventArgs e) => ViewModel.StartGame();

        private void MainView_KeyDown(object sender, KeyEventArgs e) => ViewModel.HandleKeyDown(e);

        private void Window_Closed(object sender, EventArgs e) => ViewModel.HandleClosed();

        #endregion Event Handler
    }
}
