using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris.Models {
    /// <summary>
    /// Trida reprezentuje jednotlive bloky, ze kterych se skladaji jednotlive
    /// tvary ve hre. Kazdy blok je tvoren ctyruhelnikem, ktery je obsazen v 
    /// ramecku.
    /// </summary>
    public class Block {
        /// <summary>
        /// Okraj kolem blocku
        /// </summary>
        private Border border;

        /// <summary>
        /// Barva blocku, predavana parametrem
        /// </summary>
        private Brush color;

        /// <summary>
        /// Ctyruhelnik predstavujici block
        /// </summary>
        private Rectangle rectangle;

        /// <summary>
        /// X a Y souradnice 
        /// </summary>
        private int row, column;

        /// <summary>
        /// Inicializuje barvu, border a ctyruhelnik. 
        /// Do obsahu borderu pridava ctyruhelnik.
        /// </summary>
        /// <param name="color">Zvolena barva objektu</param>
        public Block(Brush color) {
            this.color = color;

            this.border = new Border();
            this.border.Width = ImportantValues.Size;
            this.border.Height = ImportantValues.Size;
            this.border.BorderThickness = new System.Windows.Thickness(1);
            this.border.BorderBrush = Brushes.Black;

            this.rectangle = new Rectangle();
            this.rectangle.Width = ImportantValues.Size;
            this.rectangle.Height = ImportantValues.Size;
            this.rectangle.Fill = this.color;

            this.border.Child = this.rectangle;
        }

        /// <summary>
        /// Vraci border Blocku
        /// </summary>
        public Border Border
        {
            get { return this.border; }
        }

        /// <summary>
        /// Vraci vysku blocku
        /// </summary>
        public double Width
        {
            get { return this.rectangle.Width; }
        }

        /// <summary>
        /// Vraci sirku blocku
        /// </summary>
        public double Height
        {
            get { return this.rectangle.Height; }
        }

        /// <summary>
        /// Vraci ctyruhelnik predstavujici block
        /// </summary>
        public Rectangle Rectangle
        {
            get { return this.rectangle; }
        }

        /// <summary>
        /// Vraci a nastavuje X souradnici
        /// </summary>
        public int Row
        {
            get { return this.row; }
            set { this.row = value; }
        }

        /// <summary>
        /// Vraci a nastavuje Y souradnici
        /// </summary>
        public int Column
        {
            get { return this.column; }
            set { this.column = value; }
        }

        public Brush Color
        {
            get { return this.color; }
            set { this.color = value; }
        }
    }
}
