using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Tetris.Logic;

namespace Tetris.Models
{
    public class Block
    {
        #region Constructor

        public Block(Brush color)
        {
            Border = new Border
            {
                Width = ImportantValues.Size,
                Height = ImportantValues.Size,
                BorderThickness = new System.Windows.Thickness(1),
                BorderBrush = Brushes.Black
            };

            var rectangle = new Rectangle
            {
                Width = ImportantValues.Size,
                Height = ImportantValues.Size,
                Fill = color
            };

            Border.Child = rectangle;
        }

        #endregion Constructor

        #region Properties

        public Border Border { get; set; }

        public Position Position { get; set; }

        #endregion Properties
    }
}
