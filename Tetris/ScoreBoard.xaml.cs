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

namespace Tetris {
    /// <summary>
    /// Interaction logic for ScoreBoard.xaml
    /// </summary>
    public partial class ScoreBoard : Window {
        public ScoreBoard() {                
            InitializeComponent();
            this.scoreTable.ItemsSource = ConvertRecords(
                FileReader.GetInstance.Document);
        }
        
        private List<Record> ConvertRecords(XDocument document) {
            var result = from d in document.Descendants("player")
                         select d;
            var records = new List<Record>();

            foreach (var item in result) {
                var name = item.Element("playername").Value;
                var score = Convert.ToInt32(item.Element("score").Value);
                var level = Convert.ToInt32(item.Element("level").Value);
                var rows = Convert.ToInt32(item.Element("rows").Value);

                records.Add(new Record(name, score, rows, level));
            }

            return records;
        }
    }
}
