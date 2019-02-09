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
    /// Interaction logic for Naming.xaml
    /// </summary>
    public partial class NaminWindowg : Window {
        public NaminWindowg() {
            InitializeComponent();
            this.nameBox.Focus();
            this.nameBox.KeyDown += NameBox_KeyDown;
            this.confirmBtn.Click += ConfirmBtn_Click;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e) {
            CheckAndStart();
        }

        private void NameBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter)
                CheckAndStart();
        }

        private bool IsNameEmpty() {
            if (nameBox.Text.Equals(""))
                return true;
            return false;
        }

        private void StartGame(string playerName) {
            MainWindow main = new MainWindow(playerName);
            App.Current.MainWindow = main;
            main.Show();
            this.Close();
        }

        private void CheckAndStart() {
            if (IsNameEmpty())
                MessageBox.Show("Nezadali jste žádné jméno.", "Varování",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            else {
                StartGame(nameBox.Text);
            }
        }
    }
}
