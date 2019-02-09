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
    /// Interaction logic for PauseMenu.xaml
    /// </summary>
    public partial class PauseMenu : Window {
        private Game gameInstance;
        private TextBlock[] menuItems;
        private MenuManager manager;
        // barva zvyraznene polozky menu
        private Brush highlightColor = Brushes.White;

        public PauseMenu(Game gameInstance) {
            InitializeComponent();
            this.gameInstance = gameInstance;
            this.menuItems = new TextBlock[] {
                resumeTextBlock,
                restartTextBlock,
                scoreTextBlock,
                exitTextBlock
            };

            manager = new MenuManager(this, menuItems, gameInstance, 
                highlightColor);
        }
    }
}
