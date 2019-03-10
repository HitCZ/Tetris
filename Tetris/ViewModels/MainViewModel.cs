using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using Tetris.Annotations;
using Tetris.Logic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly Game gameInstance;
        private readonly BlurEffect blur;
        private const int RADIUS_COEFFICIENT = 10;
        private int currentLevel;
        private int currentScore;

        #endregion Fields

        #region Properties

        public int CurrentLevel
        {
            get => currentLevel;
            private set
            {
                currentLevel = value;
                OnPropertyChanged(nameof(CurrentLevel));
            }
        }

        public int CurrentScore
        {
            get => currentScore;
            private set
            {
                currentScore = value;
                OnPropertyChanged(nameof(CurrentScore));
            }
        }

        public Action ClearNextShapeGridAction;

        public Action<Border> AddBlockToNextShapeGridAction;

        public Action ClearGameGridAction;

        public Action<Border> AddBlockToGameGridAction;

        public Action<Border> RemoveBlockFromGameGridAction;

        #endregion Properties

        #region Constructor

        public MainViewModel(MainView mainView, int numberOfRows, int numberOfColumns, string playerName)
        {
            blur = new BlurEffect { Radius = 0 };
            mainView.Effect = blur;

            gameInstance = new Game(this, numberOfRows, numberOfColumns, playerName);
            gameInstance.GameScoreChangedAction += AdjustGameScore;
            gameInstance.GameLevelChangedAction += AdjustGameLevel;
        }

        #endregion Constructor

        #region Private Methods

        private void AdjustGameScore(int score) => CurrentScore = score;

        private void AdjustGameLevel(int level) => CurrentLevel = level;

        #endregion Private Methods

        #region Public Methods

        public void StartGame() => gameInstance.StartNewGame();

        public void HandleClosed() => Application.Current.Shutdown();

        public void HandleKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    gameInstance.PauseCommand.Execute(null);
                    break;
                case Key.Left:
                    gameInstance.MoveLeftCommand.Execute(null);
                    break;
                case Key.Right:
                    gameInstance.MoveRightCommand.Execute(null);
                    break;
                case Key.Down:
                    gameInstance.MoveDownCommand.Execute(null);
                    break;
                case Key.Space:
                    gameInstance.RotateCommand.Execute(null);
                    break;
            }
        }

        /// <summary>
        /// Adds blur.
        /// </summary>
        public void AddBlur()
        {
            blur.Radius += RADIUS_COEFFICIENT;
        }

        /// <summary>
        /// Sharpens window.
        /// </summary>
        public void Sharpen()
        {
            if (blur.Radius > 0)
                blur.Radius -= RADIUS_COEFFICIENT;
        }

        /// <summary>
        /// Removes blur.
        /// </summary>
        public void RemoveBlur()
        {
            blur.Radius = 0;
        }

        #endregion Public Methods

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
