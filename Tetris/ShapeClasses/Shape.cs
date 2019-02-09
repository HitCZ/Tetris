using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Tetris {
    /// <summary>
    /// Trida predstavuje vzor pro vsechny tvary hernich objektu
    /// </summary>
    public abstract class Shape {
        /// <summary>
        /// Pole obsahujici jednotlive casti obrazce
        /// </summary>
        private Block[,] composedShape;

        /// <summary>
        /// Blok obrazce
        /// </summary>
        private Block part1;

        /// <summary>
        /// Blok obrazce
        /// </summary>
        private Block part2;

        /// <summary>
        /// Blok obrazce
        /// </summary>
        private Block part3;

        /// <summary>
        /// Blok obrazce
        /// </summary>
        private Block part4;

        /// <summary>
        /// Barva spolecna pro cely obrazec
        /// </summary>
        private Brush color;

        /// <summary>
        /// Odkaz na herni instanci
        /// </summary>
        private Game gameInstance;

        /// <summary>
        /// Stav natoceni objektu
        /// </summary>
        private RotationState rotationState = RotationState.Default;

        protected ShapeType type;

        /// <summary>
        /// Seznam jednotlivych casti tvoricich vysledny öbjekt
        /// </summary>
        private List<Block> listOfParts;

        /// <summary>
        /// Seznam barev, ktere muze objekt mit
        /// </summary>
        private List<Color> listOfColors;

        /// <summary>
        /// Souradnice objektu
        /// </summary>
        private int row, column;

        /// <summary>
        /// Ciselna hodnota udavajici pocet moznych otoceni objektu
        /// </summary>
        private int numberOfRotationStates = 0;

        /// <summary>
        /// Inicializuje odkaz na hlavni okno a prirazuje nahodnou barvu
        /// </summary>
        /// <param name="window"></param>
        public Shape(int row, int column, Game game) {
            this.row = row;
            this.column = column;
            this.gameInstance = game;

            this.listOfParts = new List<Block>();
            this.listOfColors = new List<Color>();
            this.AddColorsToList();

            this.SetShapeColor(this.RandomColor());

            this.part1 = new Block(this.color);
            this.part2 = new Block(this.color);
            this.part3 = new Block(this.color);
            this.part4 = new Block(this.color);

            this.AddPartsToList();

            this.composedShape = new Block[4, 4];

            this.InitializeShape();
        }

        /// <summary>
        /// Inicializuje odkaz na herni instanci
        /// </summary>
        /// <param name="game">Aktualni hra</param>
        public Shape(Game game) {
            this.gameInstance = game;
        }

        /// <summary>
        /// Vraci barvu objektu
        /// </summary>
        public Brush ShapeColor
        {
            get { return this.color; }
        }

        /// <summary>
        /// vraci pole predstavujici herni objekt
        /// </summary>
        public Block[,] ComposedShape
        {
            get { return this.composedShape; }
            set { this.composedShape = value; }
        }

        /// <summary>
        /// Vraci a nastavuje blok herniho objektu
        /// </summary>
        public Block Part1
        {
            get { return this.part1; }
            set { this.part1 = value; }
        }
        
        /// <summary>
        /// Vraci a nastavuje blok herniho objektu
        /// </summary>
        public Block Part2
        {
            get { return this.part2; }
            set { this.part2 = value; }
        }

        /// <summary>
        /// Vraci a nastavuje blok herniho objektu
        /// </summary>
        public Block Part3
        {
            get { return this.part3; }
            set { this.part3 = value; }
        }

        /// <summary>
        /// Vraci a nastavuje blok herniho objektu
        /// </summary>
        public Block Part4
        {
            get { return this.part4; }
            set { this.part4 = value; }
        }

        /// <summary>
        /// Vraci a nastavuje aktualni stav rotace
        /// </summary>
        public RotationState RotationState
        {
            get { return this.rotationState; }
            set { this.rotationState = value; }
        }

        public ShapeType Type
        {
            get { return this.type; }
        }
        

        /// <summary>
        /// Vraci seznam vsech casti objektu
        /// </summary>
        public List<Block> ListOfParts
        {
           get { return this.listOfParts; }
        }

        /// <summary>
        /// Vraci vertikalni souradnici objektu
        /// </summary>
        public int Row
        {
            get
            {
                return this.row;
            }
            set
            {
                this.row = value;
            }
        }

        /// <summary>
        /// Vraci horizontalni souradnici objektu
        /// </summary>
        public int Column
        {
            get
            {
                return this.column;
            }

            set
            {
                this.column = value;
            }
        }

        /// <summary>
        /// Vraci a nastavuje pocet moznych rotaci objektu
        /// </summary>
        public int NumberOfRotationStates
        {
            get { return this.numberOfRotationStates; }
            set { this.numberOfRotationStates = value; }
        }
        
        /// <summary>
        /// Vytvori v ramci instance dany typ, 
        /// poskladanim bloku do daneho tvaru.
        /// </summary>
        protected abstract void InitializeShape();

        /// <summary>
        /// Zmeni barvu objektu na jeji zadanou hodnotu
        /// </summary>
        /// <param name="color"></param>
        public void ChangeColor(Brush color) {
            this.color = color;

            foreach (Block part in this.listOfParts) {
                part.Rectangle.Fill = this.color;
            }
        }

        /// <summary>
        /// Zmeni tloustku okraje objektu
        /// </summary>
        /// <param name="thickness"></param>
        public void ChangeBorderThickness(int thickness) {
            foreach (Block part in this.listOfParts) {
                part.Border.BorderThickness = new Thickness(thickness);
            }
        }

        /// <summary>
        /// Otoci obrazec
        /// </summary>
        public virtual void Rotate() {
            if (numberOfRotationStates == 0) {
                throw new NullReferenceException(
                    "Shape doesnt have a set number of rotation states.");
            } 
            else if (numberOfRotationStates == 2) {
                switch (this.rotationState) {
                    case RotationState.Default:
                        this.ChangeRotationToRotatedOnce();
                        break;
                    case RotationState.RotatedOnce:
                        this.ChangeRotationToDefault();
                        break;
                }
            }
            else if (this.numberOfRotationStates == 4) {
                switch (this.rotationState) {
                    case RotationState.Default:
                        this.ChangeRotationToRotatedOnce();
                        break;
                    case RotationState.RotatedOnce:
                        this.ChangeRotationToRotatedTwice();
                        break;
                    case RotationState.RotatedTwice:
                        this.ChangeRotationToRotatedThreeTimes();
                        break;
                    case RotationState.RotatedThreeTimes:
                        this.ChangeRotationToDefault();
                        break;
                }
            }
        }

        /// <summary>
        /// Otoci objekt do zadane polohy
        /// </summary>
        /// <param name="state">Zadouci poloha</param>
        public void ChangeRotationState(RotationState state) {
            switch (state) {
                case RotationState.Default:
                    this.ChangeRotationToDefault();
                    break;
                case RotationState.RotatedOnce:
                    this.ChangeRotationToRotatedOnce();
                    break;
                case RotationState.RotatedTwice:
                    this.ChangeRotationToRotatedTwice();
                    break;
                case RotationState.RotatedThreeTimes:
                    this.ChangeRotationToRotatedThreeTimes();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Zmeni rotaci objektu na vychozi
        /// </summary>
        protected abstract void ChangeRotationToDefault();

        /// <summary>
        /// Poprve otoci objekt o 90 stupnu
        /// </summary>
        protected abstract void ChangeRotationToRotatedOnce();

        /// <summary>
        /// Podruhe otoci objekt o 90 stupnu
        /// </summary>
        protected abstract void ChangeRotationToRotatedTwice();

        /// <summary>
        /// Potreti otoci objekt o 90 stupnu
        /// </summary>
        protected abstract void ChangeRotationToRotatedThreeTimes();
        
        /// <summary>
        /// Posune objekt v danem smeru o zadanou vzdalenost
        /// </summary>
        /// <param name="orientation">Orientace - horizontalni/vertikalni</param>
        /// <param name="distance">Vzdalenost posunu</param>
        public void Move(Orientation orientation, int distance) {
            switch (orientation) {
                case Orientation.Horizontal:
                    this.MoveHorizontally(distance);
                    break;
                case Orientation.Vertical:
                    this.MoveVertically(distance);
                    break;
            }
        }

        /// <summary>
        /// Posune objekt horizontalne o danou vzdalenost
        /// </summary>
        /// <param name="distance">Vzdalenost</param>
        private void MoveHorizontally(int distance) {
            this.column += distance;

            foreach (Block part in listOfParts) {
                part.Column += distance;
            }
        }
        
        /// <summary>
        /// Posune objekt na mape vertikalne o danou vzdalenost
        /// </summary>
        /// <param name="distance">Vzdalenost</param>
        private void MoveVertically(int distance) {
            this.row += distance;

            foreach (Block part in listOfParts) {
                part.Row += distance;
            }
        }

        /// <summary>
        /// Prida vsechny casti objektu do seznamu
        /// </summary>
        private void AddPartsToList() {
            this.listOfParts.Add(this.part1);
            this.listOfParts.Add(this.part2);
            this.listOfParts.Add(this.part3);
            this.listOfParts.Add(this.part4);
        }

        /// <summary>
        /// Prida vsechny barvy, ktere se ve hre vyskytuji do seznamu
        /// </summary>
        private void AddColorsToList() {
            listOfColors.Add(Color.Blue);
            listOfColors.Add(Color.Green);
            listOfColors.Add(Color.Red);
            listOfColors.Add(Color.Yellow);
            listOfColors.Add(Color.Purple);
            listOfColors.Add(Color.Pink);
            listOfColors.Add(Color.White);
            listOfColors.Add(Color.Orange);
            listOfColors.Add(Color.DarkBlue);
        }

        /// <summary>
        /// Vybere nahodnou barvu z vyctoveho typu a vrati jeho hodnotu
        /// </summary>
        /// <returns>Hodnota vyctoveho typu</returns>
        protected Color RandomColor() {
            var r = new Random();

            // nahodny index
            var i = r.Next(listOfColors.Count);

            // nahodna barva na zaklade indexu
            var randomColor = listOfColors[i];

            return randomColor;
        }

        /// <summary>
        /// Zvoli nahodnou barvu a priradi ji do promenne color
        /// </summary>
        protected virtual void SetShapeColor(Color c) {
            this.color = this.ConvertHexToBrush(this.GetColorHex());
        }

        /// <summary>
        /// Priradi zvolene barve hexadecimalni kod a tento kod vrati
        /// </summary>
        /// <returns>HEX kod barvy</returns>
        protected string GetColorHex() {
            var randomColorHex = string.Empty;
            var randomColor = this.RandomColor();

            switch (randomColor) {
                case Color.Blue:
                    randomColorHex = "#00d2ff";
                    break;
                case Color.DarkBlue:
                    randomColorHex = "#1c37ff";
                    break;
                case Color.Green:
                    randomColorHex = "#08ff4e";
                    break;
                case Color.Pink:
                    randomColorHex = "#ee2bff";
                    break;
                case Color.Yellow:
                    randomColorHex = "#f9e400";
                    break;
                case Color.Red:
                    randomColorHex = "#ff0000";
                    break;
                case Color.Orange:
                    randomColorHex = "#ff9600";
                    break;
                case Color.White:
                    randomColorHex = "#ffffff";
                    break;
                case Color.Purple:
                    randomColorHex = "#9740c9";
                    break;
            }
            return randomColorHex;
        }

        /// <summary>
        /// Prevede predany HEX kod na barvu a tu vrati
        /// </summary>
        /// <param name="colorHex">HEX kod barvy</param>
        /// <returns>Zkonvertovana barva</returns>
        protected Brush ConvertHexToBrush(string colorHex) {
            var converter = new BrushConverter();

            return (Brush)converter.ConvertFromString(colorHex);
        }
    }
}
