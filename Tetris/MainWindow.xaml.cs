using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris.Logic;

namespace Tetris {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Game gameInstance;
        private BlurEffect blur;
        private int radiusCoefficient = 10;

        public MainWindow(string name) {
            InitializeComponent();
            this.blur = new BlurEffect();
            this.blur.Radius = 0; 
            this.Effect = blur;
            gameInstance = new Game(this, name);
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            this.gameInstance.StartNewGame();
        }

        private void Window_Closed(object sender, EventArgs e) {
            App.Current.Shutdown();
        }
        
        /// <summary>
        /// O koeficient zvysi rozostreni
        /// </summary>
        public void AddBlur() {
            this.blur.Radius += radiusCoefficient;
        }

        /// <summary>
        /// Zostri okno o koeficient
        /// </summary>
        public void Sharpen() {
            if (blur.Radius > 0)
                this.blur.Radius -= radiusCoefficient;
        }

        /// <summary>
        /// Odstrani efekt rozostreni
        /// </summary>
        public void RemoveBlur() {
            blur.Radius = 0;
        }
    }
}
