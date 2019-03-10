using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using Tetris.Logic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public class MainViewModel
    {
        #region Fields

        private readonly Game gameInstance;
        private readonly BlurEffect blur;
        private const int RADIUS_COEFFICIENT = 10;

        #endregion Fields

        #region Properties

        public int CurrentLevel => gameInstance.CurrentLevel;

        public int Score => gameInstance.CurrentScore;

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
        }

        #endregion Constructor

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
    }
}
