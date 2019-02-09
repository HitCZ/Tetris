using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Tetris.Logic.Enums;
using Tetris.Logic.ShapeClasses;
using Orientation = Tetris.Logic.Enums.Orientation;

namespace Tetris
{
    public class Game {
        /// <summary>
        /// Odkaz na hlavni okno aplikace
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Zapisovac skore
        /// </summary>
        private FileWriter writer;

        /// <summary>
        /// Herni skore
        /// </summary>
        private int score = 0;

        /// <summary>
        /// Herni mapa
        /// </summary>
        private Block[,] map;

        /// <summary>
        /// Seznam vsech tvaru, ktere se ve hre mohou vyskytnout
        /// </summary>
        private List<ShapeType> listOfShapeTypes;

        /// <summary>
        /// Generator nahodnych cisel
        /// </summary>
        private Random random;

        /// <summary>
        /// Tvar, ktery hrac ovlada
        /// </summary>
        private Shape controlableShape;

        /// <summary>
        /// Pristi objekt, ktery bude do hry pridan
        /// </summary>
        private Shape nextShape;

        /// <summary>
        /// Vzdalenost, o kterou se objekt posune
        /// </summary>
        private int moveDistance = 1;

        /// <summary>
        /// Uchovava informaci o tom, zda byla zjistena kolize mez objekty
        /// </summary>
        private bool collisionDetected;

        /// <summary>
        /// Seznam jednotlivych ctverecku ve hre
        /// </summary>
        private List<Block> listOfBlocks;

        /// <summary>
        /// Casovac zajistujici posun objektu dolu za urcity cas
        /// </summary>
        private DispatcherTimer dropTimer;

        private string playerName;

        /// <summary>
        /// Radek pro spawn
        /// </summary>
        private int row = 2;

        /// <summary>
        /// Policko pro spawn
        /// </summary>
        private int column = 4;

        /// <summary>
        /// Cislo aktualniho levelu
        /// </summary>
        private int level = 1;

        /// <summary>
        /// Kolik radku bylo odstraneno za sebou
        /// </summary>
        private int streakOfClearedRows = 0;

        /// <summary>
        /// Kolik radku bylo odstraneno celkem
        /// </summary>
        private int totalNumberOfClearedRows = 0;

        /// <summary>
        /// Maximalni pocet odstranenych radku, pred postupem do dalsi urovne
        /// </summary>
        private int clearedRowsLimit = 15;

        /// <summary>
        /// Pocatecni interval automatickeho posunu objektu
        /// </summary>
        private int startingTimerInterval = 750;

        /// <summary>
        /// Aktualni interval posunu objektu dolu
        /// </summary>
        private int currentDropTimerInterval;


        private bool paused = false;

        /// <summary>
        /// Inicializuje herni instanci
        /// </summary>
        public Game() {
            this.InitializeInstance();
        }

        /// <summary>
        /// Inicializuje odkaz na hlavni okno, DataContext pro skore TextBlock a
        /// instancni promenne. Odstartuje novou hru
        /// </summary>
        /// <param name="mainWindow"></param>
        public Game(MainWindow mainWindow, string name) {
            this.mainWindow = mainWindow;
            this.mainWindow.txtScoreValue.DataContext = this;
            this.mainWindow.txtLevelValue.DataContext = this;
            this.playerName = name;

            this.InitializeInstance();
            this.StartNewGame();
            FileWriter w = new FileWriter();

        }

        public string PlayerName
        {
            get
            {
                return this.playerName;
            }
        }
        /// <summary>
        /// Vraci odkaz na herni mapu
        /// </summary>
        public Block[,] Map
        {
            get { return this.map; }
        }

        /// <summary>
        /// Vraci a nastavuje skore, pri nastaveni aktualizuje TextBlock
        /// </summary>
        public int Score
        {
            get
            {
                return this.score;
            }

            set
            {
                this.score = value;
                this.mainWindow.txtScoreValue.Text = this.score.ToString();
            }
        }

        /// <summary>
        /// vraci a nastavuje konkretni cislo urovne
        /// </summary>
        public int Level
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
                this.mainWindow.txtLevelValue.Text = this.level.ToString();
            }
        }

        public int ClearedRows
        {
            get
            {
                return this.totalNumberOfClearedRows;
            }
        }

        public bool IsPaused
        {
            get
            {
                return this.paused;
            }
        }
        /// <summary>
        /// Spusti novou hru
        /// </summary>
        public void StartNewGame() {
            this.Reset();
            this.mainWindow.RemoveBlur();
            this.controlableShape =
               //this.SpawnContreteShape(ShapeType.ShapeLInverted, 5, 5, this);
               this.SpawnNewShape(this.row, this.column);
            this.AddShapeToGame(this.controlableShape);

            this.nextShape = this.SpawnNewShape(this.row, this.column);
            this.AddShapeToNextShapeGrid(this.nextShape);

            this.dropTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeInstance() {
            this.random = new Random();
            this.listOfShapeTypes = new List<ShapeType>();
            this.AddShapeTypesToList();
            this.mainWindow.KeyDown += PlayerInput;
            this.listOfBlocks = new List<Block>();
            this.currentDropTimerInterval = this.startingTimerInterval;

            // pocet radku v gridu
            var numberOfRows =
                this.mainWindow.gameGrid.RowDefinitions.Count;

            // pocet sloupcu v gridu
            var numberOfColumns =
                this.mainWindow.gameGrid.ColumnDefinitions.Count;

            map = new Block[numberOfRows, numberOfColumns];

            this.DropTimerInit();
        }

        private void ShowMainMenu() {
            PauseMenu pauseMenu = new PauseMenu(this);
            pauseMenu.Show();
        }

        /// <summary>
        /// Resetuje vychozi hodnoty hry
        /// - vycisteni mapy
        /// - vynulovani skore
        /// - navrat do prvni urovne
        /// </summary>
        private void Reset() {
            this.Score = 0;
            this.totalNumberOfClearedRows = 0;
            this.streakOfClearedRows = 0;
            this.Level = 1;
            this.dropTimer.Stop();
            this.listOfBlocks.Clear();
            this.paused = false;
            this.ClearMap();
        }

        /// <summary>
        /// Zapauzuje hru a zobrazi pauzovaci nabidku
        /// </summary>
        public void Pause() {
            this.paused = true;
            this.dropTimer.Stop();
            this.mainWindow.AddBlur();
        }

        /// <summary>
        /// Pokracovani ve hre
        /// </summary>
        public void Resume() {
            this.paused = false;
            this.mainWindow.RemoveBlur();
            this.dropTimer.Start();
        }

        /// <summary>
        /// Ukonci aplikaci
        /// </summary>
        public void Exit() {
            App.Current.Shutdown();
        }

        /// <summary>
        /// Vytvori zaznam v XML souboru
        /// </summary>
        public void CreateRecord() {
            Record r = new Record(
                this.playerName,
                this.score, 
                this.totalNumberOfClearedRows, 
                this.level);

            if (writer == null)
                this.writer = new FileWriter();
            this.writer.Write(r);
        }
        /// <summary>
        /// Postup do dalsi urovne, zrychleni posouvani objektu
        /// </summary>
        private void LevelUp() {
            this.totalNumberOfClearedRows = 0;
            this.streakOfClearedRows = 0;
            this.Level++;
            this.listOfBlocks.Clear();
            this.ClearMap();

            if (this.currentDropTimerInterval > 50) {
                this.currentDropTimerInterval -= 50;
            }
            else if (this.currentDropTimerInterval == 50) {
                this.currentDropTimerInterval -= 49;
            }
            else if (this.currentDropTimerInterval <= 1) {
                this.GameOver();
            }

            this.dropTimer.Interval =
                TimeSpan.FromMilliseconds(this.currentDropTimerInterval);

            Console.WriteLine("Interval: " + this.dropTimer.Interval);
        }

        /// <summary>
        /// Prida Shape do seznamu hernich objektu
        /// </summary>
        /// <param name="shape">Pridavany objekt</param>
        public void AddBlocksToList(Shape shape) {
            foreach (Block block in shape.ListOfParts) {
                this.listOfBlocks.Add(block);
            }
        }

        /// <summary>
        /// Odstrani zadany shape ze seznamu hernich objektu
        /// </summary>
        /// <param name="shape">Odebirany objekt</param>
        public void RemoveGameObject(Shape shape) {
            foreach (Block block in shape.ListOfParts) {
                this.listOfBlocks.Remove(block);
            }
        }

        /// <summary>
        /// Generuje nahodne cislo radku
        /// </summary>
        /// <returns>Nahodne cislo radku</returns>
        private int RandomColumn() {
            return random.Next(1, map.GetLength(1) - 3);
        }

        private void ClearMap() {
            for (int i = 0; i < this.map.GetLength(0); i++) {
                for (int j = 0; j < this.map.GetLength(1); j++) {
                    this.map[i, j] = null;
                }
            }
        }

        private void AddShapeToNextShapeGrid(Shape shape) {
            this.mainWindow.gridNextShape.Children.Clear();

            if (shape != null) {
                for (int row = 0; row < shape.ComposedShape.GetLength(0); row++) {
                    for (int col = 0; col < shape.ComposedShape.GetLength(1); col++) {
                        if (shape.ComposedShape[row, col] != null) {
                            Grid.SetRow(shape.ComposedShape[row, col].Border, row);
                            Grid.SetColumn(shape.ComposedShape[row, col].Border, col);
                            this.mainWindow.gridNextShape.Children.Add(shape.ComposedShape[row, col].Border);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inicializace casovace, ktery zajistuje padani objektu
        /// </summary>
        private void DropTimerInit() {
            this.dropTimer = new DispatcherTimer(DispatcherPriority.Render);
            this.dropTimer.Interval = TimeSpan.FromMilliseconds(this.startingTimerInterval);
            this.dropTimer.Tick += DropTimer_Tick;
        }

        /// <summary>
        /// V kazdem ticku se posune objekt na danou souradnici
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropTimer_Tick(object sender, EventArgs e) {
            this.MoveShape(this.controlableShape, Orientation.Vertical, this.moveDistance);
        }

        /// <summary>
        /// Zpracovava vstup od hrace, pokud zmackne jednu z povolenych klaves,
        /// odstrani se aktualni obrazec, provede se odpovidajici akce a obrazec
        /// se opet prida do mapy, nasledne se aktualizuje herni obrazovka.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerInput(object sender, KeyEventArgs e) {
            if (!paused) {
                if (this.controlableShape != null) {
                    switch (e.Key) {
                        case Key.Space:
                            this.RotateShape(this.controlableShape);
                            break;
                        // posune objekt vertikalne o zapornou vzdalenost
                        /*
                        case Key.Up:
                                this.MoveShape(
                                    this.controlableShape,
                                    Orientation.Vertical,
                                    -this.moveDistance);
                            
                            break;
                            */
                        case Key.Right:
                            this.MoveShape(
                                this.controlableShape,
                                Orientation.Horizontal,
                                this.moveDistance);
                            break;
                        case Key.Down:
                            this.MoveShape(
                                this.controlableShape,
                                Orientation.Vertical,
                                this.moveDistance);
                            break;
                        case Key.Left:
                            // posune objekt horizontalne o zapornou vzdalenost
                            this.MoveShape(
                                this.controlableShape,
                                Orientation.Horizontal,
                                -this.moveDistance);
                            break;
                        case Key.Escape:
                            this.Pause();
                            this.ShowMainMenu();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Aktualizuje hru a herni mapu
        /// </summary>
        private void UpdateGame() {
            this.RefreshGrid();
        }

        /// <summary>
        /// Do herni mrizky priradi bordery jednotlivych bloku
        /// </summary>
        private void RefreshGrid() {
            this.mainWindow.gameGrid.Children.Clear();

            foreach (Block block in listOfBlocks) {
                Grid.SetRow(block.Border, block.Row);
                Grid.SetColumn(block.Border, block.Column);

                this.mainWindow.gameGrid.Children.Add(block.Border);

            }
        }

        /// <summary>
        /// Naplni seznam vsemi tvary, ktere se mohou ve hre vysktnout
        /// </summary>
        private void AddShapeTypesToList() {
            this.listOfShapeTypes.Add(ShapeType.ShapeL);
            this.listOfShapeTypes.Add(ShapeType.ShapeLInverted);
            this.listOfShapeTypes.Add(ShapeType.ShapeI);
            this.listOfShapeTypes.Add(ShapeType.ShapeO);
            this.listOfShapeTypes.Add(ShapeType.ShapeZ);
            this.listOfShapeTypes.Add(ShapeType.ShapeS);
            this.listOfShapeTypes.Add(ShapeType.ShapeT);
        }

        /// <summary>
        /// Vygeneruje soucasny a nasledujici objekt a prida je do odpovidajicich
        /// gridu
        /// </summary>
        private void SpawnShapes() {
            this.mainWindow.gridNextShape.Children.Clear();
            this.controlableShape = this.nextShape;
            this.AddShapeToGame(this.controlableShape);
            this.nextShape = this.SpawnNewShape(this.row, this.column);
            this.AddShapeToNextShapeGrid(this.nextShape);
        }

        /// <summary>
        /// Prida do hry novy herni objekt
        /// </summary>
        private Shape SpawnNewShape(int row, int column) {
            Console.WriteLine("X: {0} : Y: {1}", column, row);
            Console.WriteLine("New shape spawned");

            // vytvori nahodny objekt
            Shape newShape = this.RandomShape(row, column, this);
            this.ChooseRandomRotation(newShape);

            return newShape;
        }

        /// <summary>
        /// Nekolikrat (nahodne) zrotuje objekt.
        /// </summary>
        /// <param name="shape"></param>
        private void ChooseRandomRotation(Shape shape) {
            if (shape != null) {
                var range = random.Next(10);
                // na zaklade nahodneho cisla zrotuje objekt
                for (int i = 0; i < range; i++) {
                    shape.Rotate();
                }
            }
        }

        /// <summary>
        /// Prida objekt do hry a hru aktualizuje
        /// </summary>
        /// <param name="shape"></param>
        private void AddShapeToGame(Shape shape) {
            if (shape != null) {
                this.AddBlocksToList(shape);
            }
            this.UpdateGame();
        }

        /// <summary>
        /// Na konkretni pozici naspawnuje konkretni objekt, 
        /// jehoz typ je zadany parametrem.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private Shape SpawnConcreteShape(ShapeType type, int row, int column, Game game) {
            Shape newShape = null;

            switch (type) {
                case ShapeType.ShapeL:
                    newShape = new ShapeL(row, column, game);
                    break;
                case ShapeType.ShapeLInverted:
                    newShape = new ShapeLInverted(row, column, game);
                    break;
                case ShapeType.ShapeI:
                    newShape = new ShapeI(row, column, game);
                    break;
                case ShapeType.ShapeO:
                    newShape = new ShapeO(row, column, game);
                    break;
                case ShapeType.ShapeZ:
                    newShape = new ShapeZ(row, column, game);
                    break;
                case ShapeType.ShapeS:
                    newShape = new ShapeS(row, column, game);
                    break;
                case ShapeType.ShapeT:
                    newShape = new ShapeT(row, column, game);
                    break;
            }

            return newShape;
        }

        /// <summary>
        /// vytvori na zadane souradnici a v aktualni hre nahodny objekt
        /// </summary>
        /// <param name="row">Radek, kde se ma objekt vytvorit</param>
        /// <param name="column">Sloupec, kde se ma objekt vytvorit</param>
        /// <param name="game">Aktualni hra</param>
        /// <returns></returns>
        private Shape RandomShape(int row, int column, Game game) {
            // nahodny index
            var i = random.Next(listOfShapeTypes.Count);

            // nahodny typ na zaklade indexu
            var randomShapeType = listOfShapeTypes[i];
            Shape randomShape = null;

            switch (randomShapeType) {
                case ShapeType.ShapeL:
                    randomShape = new ShapeL(row, column, game);
                    break;
                case ShapeType.ShapeLInverted:
                    randomShape = new ShapeLInverted(row, column, game);
                    break;
                case ShapeType.ShapeI:
                    randomShape = new ShapeI(row, column, game);
                    break;
                case ShapeType.ShapeO:
                    randomShape = new ShapeO(row, column, game);
                    break;
                case ShapeType.ShapeZ:
                    randomShape = new ShapeZ(row, column, game);
                    break;
                case ShapeType.ShapeS:
                    randomShape = new ShapeS(row, column, game);
                    break;
                case ShapeType.ShapeT:
                    randomShape = new ShapeT(row, column, game);
                    break;
            }
            return randomShape;
        }

        private void AddBlockToMap(Block block) {
            this.map[block.Row, block.Column] = block;
        }


        private void RemoveBlockFromMap(Block block) {
            this.map[block.Row, block.Column] = null;
        }

        private void RemoveBlockFromGrid(Block block) {
            this.mainWindow.gameGrid.Children.Remove(block.Border);
        }

        private void RemoveBlock(Block block) {
            this.RemoveBlockFromMap(block);
            this.RemoveBlockFromGrid(block);
        }

        /// <summary>
        /// Vezme predany objekt, a na odpovidajici pozice na mape vlozi jeho 
        /// jednotlive casti
        /// </summary>
        /// <param name="shape">Objekt pro pridani do mapy</param>
        private void AddShapeToMap(Shape shape) {
            foreach (Block part in shape.ListOfParts) {
                this.map[part.Row, part.Column] = part;
            }
        }

        /// <summary>
        /// Odstrani obrazec z herni mapy, tak ze jednotlive souradnice nastavi 
        /// na NULL
        /// </summary>
        /// <param name="shape">Obrazec pro odstraneni</param>
        private void RemoveShapeFromMap(Shape shape) {
            foreach (Block part in shape.ListOfParts) {
                this.map[part.Row, part.Column] = null;
            }
        }

        /// <summary>
        /// Odstrani dany obrazec z herni obrazovky
        /// </summary>
        /// <param name="shape">Obrazec pro odstraneni</param>
        private void RemoveShapeFromGrid(Shape shape) {
            foreach (Block part in shape.ListOfParts) {
                this.mainWindow.gameGrid.Children.Remove(part.Border);
            }
        }

        /// <summary>
        /// Odstrani zadany obrazec z herni mapy a herni obrazovky
        /// </summary>
        /// <param name="shape">Obrazec pro odstraneni</param>
        private void RemoveShape(Shape shape) {
            this.RemoveShapeFromMap(shape);
            this.RemoveShapeFromGrid(shape);
        }

        /// <summary>
        /// Zkotroluje, zda nedoslo ke kolizi, pokud ne, 
        /// odstrani puvodni objekt z mapy, zmeni jeho pozici a opet ho do mapy 
        /// prida.
        /// </summary>
        /// <param name="shape">Objekt pro posun</param>
        /// <param name="orientation">Orientace - vertikalni/horizontalni</param>
        /// <param name="distance">Vzdalenost posunu</param>
        private void MoveShape(Shape shape, Orientation orientation, int distance) {
            if (shape != null) {
                this.CheckMovementCollision(shape, orientation, distance);

                if (!collisionDetected) {
                    this.RemoveShape(shape);
                    shape.Move(orientation, distance);
                    this.AddShapeToMap(shape);
                    this.UpdateGame();
                }
                Console.WriteLine("MAX ROW: " + (this.map.GetLength(0) - 1));
                Console.WriteLine("ROW: " + shape.Row + " Column: " + shape.Column);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        private void CheckMovementCollision(Shape shape, Orientation orientation, int distance) {
            this.collisionDetected = false;

            foreach (Block part in shape.ListOfParts) {
                switch (orientation) {
                    case Orientation.Horizontal:
                        if (((part.Column + distance) < 0)
                            || (part.Column + distance)
                            > this.map.GetLength(1) - 1) {
                            this.collisionDetected = true;
                            break;
                        }
                        else if (this.map[part.Row, part.Column + distance] != null) {
                            if ((this.map[part.Row, part.Column + distance] != shape.Part1)
                                && (this.map[part.Row, part.Column + distance] != shape.Part2)
                                && (this.map[part.Row, part.Column + distance] != shape.Part3)
                                && (this.map[part.Row, part.Column + distance] != shape.Part4)) {
                                this.collisionDetected = true;

                                break;
                            }
                        }
                        break;
                    case Orientation.Vertical:
                        if ((part.Row + distance) < 0) {
                            this.collisionDetected = true;
                            break;
                        }
                        // kolize se spodnim krajem obrazovky
                        else if (
                            (part.Row + distance) > this.map.GetLength(0) - 1) {
                            this.collisionDetected = true;
                            this.CheckRows();
                            this.CalculateScore();
                            this.SpawnShapes();
                            return;
                        }
                        else if
                            (this.map[(part.Row + distance), part.Column] != null) {
                            if ((this.map[part.Row + distance, part.Column] != shape.Part1)
                                && (this.map[part.Row + distance, part.Column] != shape.Part2)
                                && (this.map[part.Row + distance, part.Column] != shape.Part3)
                                && (this.map[part.Row + distance, part.Column] != shape.Part4)) {
                                this.collisionDetected = true;

                                this.CheckRows();
                                this.CalculateScore();
                                this.SpawnShapes();

                                return;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Zkontroluje, zda po rotaci nezasahuje objekt mimo herni obrazovku
        /// </summary>
        /// <param name="shape"></param>
        private void CheckRotationCollision(Shape shape) {
            this.collisionDetected = false;

            foreach (Block part in shape.ListOfParts) {
                if ((part.Row < 0)
                    || (part.Row > map.GetLength(0) - 1)) {
                    this.collisionDetected = true;
                    break;
                }
                else if (part.Column < 0) {
                    this.collisionDetected = true;
                }
                else if (part.Column >= map.GetLength(1) - 1) {
                    this.collisionDetected = true;
                }
                else if (this.map[part.Row, part.Column] != null) {
                    if ((this.map[part.Row, part.Column] != shape.Part1)
                        && (this.map[part.Row, part.Column] != shape.Part2)
                        && (this.map[part.Row, part.Column] != shape.Part3)
                        && (this.map[part.Row, part.Column] != shape.Part4)) {
                        this.collisionDetected = true;

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Odstrani zadany objekt z mapy, otoci ho, zkontroluje, zda nedochazi
        /// ke kolizi a opet ho prida do mapy, nakonec aktualizuje hru
        /// </summary>
        /// <param name="shape"></param>
        private void RotateShape(Shape shape) {
            if (shape != null) {
                var previousRotation = shape.RotationState;

                this.RemoveShape(shape);

                shape.Rotate();
                this.CheckRotationCollision(shape);

                // pokud objekt zasahuje mimo herni pole, vrati se puvodni stav
                if (collisionDetected) {
                    shape.ChangeRotationState(previousRotation);
                }

                this.AddShapeToMap(shape);
                this.UpdateGame();
            }
        }

        private void CheckRows() {
            this.CheckTopRow();
            this.CheckFullRows();

            if (this.totalNumberOfClearedRows >= clearedRowsLimit) {
                this.LevelUp();
            }
        }

        /// <summary>
        /// Zkontroluje jestli nektery block na herni mape nezasahuje na prvni 
        /// radek mimo herni obrazovku, pokud ano, vyvola konec hry
        /// </summary>
        private void CheckTopRow() {
            var row = 5;

            for (int col = 0; col < this.map.GetLength(1); col++) {
                if (this.map[row, col] != null) {
                    Console.WriteLine("Ending game: ROW: {0} COL: {1}", row, col);
                    this.GameOver();
                }
            }
        }

        /// <summary>
        /// Zkontroluje, zda je radek zaplnen
        /// </summary>
        private void CheckFullRows() {
            var isRowFull = true;
            for (int row = this.map.GetLength(0) - 1; row >= 0; row--) {
                isRowFull = true;
                for (int column = 0; column < this.map.GetLength(1); column++) {
                    if (map[row, column] == null) {
                        isRowFull = false;
                        break;
                    }
                }
                // radek je plny
                if (isRowFull) {
                    this.DeleteRow(row);
                    this.DropRows(row);
                    this.CheckRows();
                }
            }
            Console.WriteLine("Row: {0} is full: {1}", row, isRowFull);
        }

        /// <summary>
        /// Vymaze zadany radek
        /// </summary>
        /// <param name="rowNumber"></param>
        private void DeleteRow(int rowNumber) {
            for (int column = 0; column < this.map.GetLength(1); column++) {
                this.listOfBlocks.Remove(this.map[rowNumber, column]);
                this.map[rowNumber, column] = null;
            }

            this.streakOfClearedRows++;
            this.totalNumberOfClearedRows++;
        }

        /// <summary>
        /// Vymaze radky z mapy, az do zadaneho radku
        /// </summary>
        /// <param name="rowNumber"></param>
        private void DeleteRowsUntil(int rowNumber) {
            for (int row = 0; row < rowNumber; row++) {
                for (int col = 0; col < map.GetLength(1); col++) {
                    this.map[row, col] = null;
                }
            }
        }

        /// <summary>
        /// Posune smerem dolu vsechny radky nad smazanym
        /// </summary>
        private void DropRows(int rowNumber) {
            this.ClearMap();

            foreach (Block block in this.listOfBlocks) {
                if (block.Row < rowNumber) {
                    block.Row++;
                }
                this.AddBlockToMap(block);
            }
        }

        /// <summary>
        /// Vypocitava skore, na zaklade najednou odstranenych radku
        /// </summary>
        /// <param name="numberOfDroppedRows"></param>
        private void CalculateScore() {
            var value = 0;

            switch (this.streakOfClearedRows) {
                case 1:
                    value = 40;
                    break;
                case 2:
                    value = 100;
                    break;
                case 3:
                    value = 300;
                    break;
                case 4:
                    value = 1200;
                    break;
            }
            this.Score += (value * this.level);
            this.streakOfClearedRows = 0;
        }

        private void GameOver() {
            this.CreateRecord();
            this.Pause();
            this.ShowNewGameDialog();
        }

        /// <summary>
        /// Zepta se hrace, jestli chce zacit novou hru
        /// </summary>
        private void ShowNewGameDialog() {
            var result = this.ShowDialog(
                "Konec hry, přejete si začít znovu?", "Konec hry");

            switch (result) {
                case MessageBoxResult.Yes:
                    this.StartNewGame();
                    return;
                case MessageBoxResult.No:
                    this.ShowEndGameDialog();
                    break;
            }
        }

        /// <summary>
        /// Zepta se hrace, jestli chce hru vypnout
        /// </summary>
        private void ShowEndGameDialog() {
            var result = this.ShowDialog(
                        "Přejete si hru vypnout?", "Konec hry");

            switch (result) {
                case MessageBoxResult.Yes:
                    this.Exit();
                    break;
                case MessageBoxResult.No:
                    GameOver();
                    return;
            }
        }

        private MessageBoxResult ShowDialog(string msg, string title) {
            return MessageBox.Show(msg, title, MessageBoxButton.YesNo);
        }
    }
}
