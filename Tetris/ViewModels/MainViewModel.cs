using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public int Score => gameInstance.Score;

        #endregion Properties

        #region Constructor

        public MainViewModel(MainView mainView, string playerName)
        {
            blur = new BlurEffect { Radius = 0 };
            mainView.Effect = blur;
            gameInstance = new Game(mainView, playerName);
        }

        #endregion Constructor

        #region Public Methods

        public void StartGame() => gameInstance.StartNewGame();

        public void HandleClosed() => Application.Current.Shutdown();

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
