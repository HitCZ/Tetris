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
using System.Xml.Linq;
using Tetris.Classes;

namespace Tetris {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainMenu : Window {
        private TextBlock[] menuItems;
        private MenuManager manager;

        // barva zvyraznene polozky menu
        private Brush highlightColor = Brushes.Blue;

        public MainMenu() {
            InitializeComponent();
            menuItems = new TextBlock[] {
                startTextBlock,
                scoreTextBlock,
                exitTextBlock
            };

            manager = new MenuManager(this, menuItems, this.highlightColor);            
        }
    }
}
