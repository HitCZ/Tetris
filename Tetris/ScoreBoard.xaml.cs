using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Tetris.Classes;
using Tetris.ViewModels;

namespace Tetris {
    /// <summary>
    /// Interaction logic for ScoreBoard.xaml
    /// </summary>
    public partial class ScoreBoard {

        #region Properties
        
        public ScoreBoardViewModel ViewModel
        {
            get => DataContext as ScoreBoardViewModel;
            set => DataContext = value;
        }

        #endregion Properties

        public ScoreBoard() {                
            InitializeComponent();
            ViewModel = new ScoreBoardViewModel();
        }
    }
}
