using System;
using System.Windows;
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

        public MainView(string playerName)
        {
            InitializeComponent();
            ViewModel = new MainViewModel(this, playerName);
            DataContext = ViewModel;
            Loaded += MainView_Loaded;
        }

        #region Event Handlers

        private void MainView_Loaded(object sender, RoutedEventArgs e) => ViewModel.StartGame();

        private void Window_Closed(object sender, EventArgs e) => ViewModel.HandleClosed();

        #endregion Event Handler
    }
}
