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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tetris {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window {
        public Window1() {
            InitializeComponent();
        }

        private void startTextBlockMouseEnter(object sender, MouseEventArgs e) {
            startTextBlock.Foreground = Brushes.Blue;
        }

        private void startTextBlock_MouseLeave(object sender, MouseEventArgs e) {
            startTextBlock.Foreground = Brushes.Black;
        }

        private void startTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MainWindow main = new MainWindow(this);
            App.Current.MainWindow = main;
            main.Show();
            this.Hide();
        }

        private void exitTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            App.Current.Shutdown();
        }

        private void exitTextBlock_MouseEnter(object sender, MouseEventArgs e) {
            exitTextBlock.Foreground = Brushes.Blue;
        }

        private void exitTextBlock_MouseLeave(object sender, MouseEventArgs e) {
            exitTextBlock.Foreground = Brushes.Black;
        }

        private void scoreTextBlock_MouseEnter(object sender, MouseEventArgs e) {
            scoreTextBlock.Foreground = Brushes.Blue;
        }

        private void scoreTextBlock_MouseLeave(object sender, MouseEventArgs e) {
            scoreTextBlock.Foreground = Brushes.Black;
        }

        private void scoreTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

        }
    }
}
