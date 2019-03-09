using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tetris.Annotations;
using Tetris.Logic;
using Tetris.Logic.Extensions;

namespace Tetris.ViewModels
{
    public class NamingViewModel : INotifyPropertyChanged
    {
        #region Fields

        private string playerName;

        #endregion Fields

        #region Properties

        public string PlayerName
        {
            get => playerName;
            set
            {
                playerName = value;
                OnPropertyChanged(nameof(PlayerName));
            }
        }

        /// <summary>
        /// Used to close the parent Window.
        /// </summary>
        public Action CloseAction;

        #endregion Properties

        #region Commands

        public ICommand ConfirmCommand { get; set; }

        #endregion Commands

        #region Constructor

        public NamingViewModel()
        {
            ConfirmCommand = new RelayCommand(ConfirmCommandExecute, ConfirmCommandCanExecute);
            PlayerName = "Player";
        }

        #endregion Constructor


        #region Private Methods

        private void ConfirmCommandExecute() => StartGame(PlayerName);

        private bool ConfirmCommandCanExecute() => !PlayerName.IsNullOrEmpty();

        private void StartGame(string player)
        {
            var mainWindow = new MainWindow(player);
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            CloseAction();
        }

        #endregion Private Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}
