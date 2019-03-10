using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Tetris.Annotations;
using Tetris.Logic.Enums;
using Tetris.Models;
using Tetris.Models.Shapes;
using Tetris.ViewModels;
using Tetris.Views;
using Orientation = Tetris.Logic.Enums.Orientation;

namespace Tetris.Logic
{
    public class Game : INotifyPropertyChanged
    {
        #region Fields

        private MainViewModel mainViewModel;
        private FileWriter writer;
        private int currentCurrentScore = 0;
        private Block[,] map;
        private List<ShapeType> listOfShapeTypes;
        private Random random;
        private Shape controlableShape;
        private Shape nextShape;
        private int moveDistance = 1;
        private bool collisionDetected;
        private List<Block> listOfBlocks;
        private DispatcherTimer dropTimer;
        //private string playerName;
        private int row = 2;
        private int column = 4;
        private int currentLevel = 1;
        private int streakOfClearedRows = 0;
        private int totalNumberOfClearedRows;
        private int clearedRowsLimit = 15;
        private int startingTimerInterval = 750;
        private int currentDropTimerInterval;
        private int numberOfRows;
        private int numberOfColumns;
        private bool paused;
        private string playerName;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets 
        /// </summary>
        //public Block[,] Map { get; private set; }

        /// <summary>
        /// Gets or sets the current score.
        /// </summary>
        public int CurrentScore
        {
            get => currentCurrentScore;

            set
            {
                currentCurrentScore = value;
                GameScoreChangedAction(value);
                OnPropertyChanged(nameof(CurrentScore));
                //mainWindow.txtScoreValue.Text = currentCurrentScore.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the current level.
        /// </summary>
        public int CurrentLevel
        {
            get => currentLevel;
            set
            {
                currentLevel = value;
                GameLevelChangedAction(value);
                //mainWindow.txtLevelValue.Text = currentLevel.ToString();
                OnPropertyChanged(nameof(CurrentLevel));
            }
        }

        /// <summary>
        /// Gets or sets the total number of cleared rows.
        /// </summary>
        public int ClearedRows
        {
            get => totalNumberOfClearedRows;
            private set
            {
                totalNumberOfClearedRows = value;
                OnPropertyChanged(nameof(ClearedRows));
            }
        }

        /// <summary>
        /// Gets or sets the information about the game being paused.
        /// </summary>
        public bool IsPaused
        {
            get => paused;
            private set
            {
                paused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }

        public Action<int> GameScoreChangedAction;

        public Action<int> GameLevelChangedAction;

        #endregion Properties

        #region Commands

        public ICommand MoveLeftCommand { get; set; }
        public ICommand MoveRightCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }
        public ICommand RotateCommand { get; set; }
        public ICommand PauseCommand { get; set; }

        #endregion Commands

        #region Constructor

        public Game()
        {
            Initialize();
            InitializeCommands();
        }

        /// <summary/>
        public Game(MainViewModel mainViewModel, int numberOfRows, int numberOfColumns, string playerName)
        {
            this.mainViewModel = mainViewModel;
            //this.mainViewModel.Score = Score;
            //this.mainWindow.txtLevelValue.DataContext = this;
            this.numberOfRows = numberOfRows;
            this.numberOfColumns = numberOfColumns;
            this.playerName = playerName;

            Initialize();
            InitializeCommands();
            //StartNewGame();
            //var w = new FileWriter();

        }

        #endregion Constructor

        #region Private Methods

        private void InitializeCommands()
        {
            MoveLeftCommand = new RelayCommand(MoveLeftCommandExecute);
            MoveRightCommand = new RelayCommand(MoveRightCommandExecute);
            MoveDownCommand = new RelayCommand(MoveDownCommandExecute);
            RotateCommand = new RelayCommand(RotateCommandExecute);
            PauseCommand = new RelayCommand(PauseCommandExecute);
        }

        private void MoveLeftCommandExecute()
        {
            if (paused || controlableShape is null)
                return;

            MoveShape(controlableShape, Orientation.Horizontal, -moveDistance);
        }

        private void MoveRightCommandExecute()
        {
            if (paused || controlableShape is null)
                return;

            MoveShape(controlableShape, Orientation.Horizontal, moveDistance);
        }

        private void MoveDownCommandExecute()
        {
            if (paused || controlableShape is null)
                return;

            MoveShape(controlableShape, Orientation.Vertical, moveDistance);
        }

        private void RotateCommandExecute()
        {
            if (paused || controlableShape is null)
                return;

            RotateShape(controlableShape);
        }

        private void PauseCommandExecute()
        {
            Pause();
            ShowMainMenu();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Initialize()
        {
            random = new Random();
            listOfShapeTypes = new List<ShapeType>();
            AddShapeTypesToList();
            //mainWindow.KeyDown += PlayerInput;
            listOfBlocks = new List<Block>();
            currentDropTimerInterval = startingTimerInterval;

            //// pocet radku v gridu
            //var numberOfRows = mainWindow.gameGrid.RowDefinitions.Count;

            //// pocet sloupcu v gridu
            //var numberOfColumns = mainWindow.gameGrid.ColumnDefinitions.Count;

            map = new Block[numberOfRows, numberOfColumns];

            DropTimerInit();
        }

        private void ShowMainMenu()
        {
            var pauseMenu = new PauseMenuView(this);
            pauseMenu.Show();
        }

        /// <summary>
        /// Resetuje vychozi hodnoty hry
        /// - vycisteni mapy
        /// - vynulovani skore
        /// - navrat do prvni urovne
        /// </summary>
        private void Reset()
        {
            CurrentScore = 0;
            totalNumberOfClearedRows = 0;
            streakOfClearedRows = 0;
            CurrentLevel = 1;
            dropTimer.Stop();
            listOfBlocks.Clear();
            paused = false;
            ClearMap();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Starts new game
        /// </summary>
        public void StartNewGame()
        {
            Reset();
            mainViewModel.RemoveBlur();
            controlableShape = SpawnNewShape(row, column);
            AddShapeToGame(controlableShape);

            nextShape = SpawnNewShape(row, column);
            AddShapeToNextShapeGrid(nextShape);

            dropTimer.Start();
        }

        #endregion Public Methods

        

        /// <summary>
        /// Zapauzuje hru a zobrazi pauzovaci nabidku
        /// </summary>
        public void Pause()
        {
            paused = true;
            dropTimer.Stop();
            mainViewModel.AddBlur();
        }

        /// <summary>
        /// Pokracovani ve hre
        /// </summary>
        public void Resume()
        {
            paused = false;
            mainViewModel.RemoveBlur();
            dropTimer.Start();
        }

        /// <summary>
        /// Ukonci aplikaci
        /// </summary>
        public void Exit()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Vytvori zaznam v XML souboru
        /// </summary>
        public void CreateRecord()
        {
            var r = new Record(
                playerName,
                currentCurrentScore,
                totalNumberOfClearedRows,
                currentLevel);

            if (writer == null)
                writer = new FileWriter();
            writer.Write(r);
        }
        /// <summary>
        /// Postup do dalsi urovne, zrychleni posouvani objektu
        /// </summary>
        private void LevelUp()
        {
            totalNumberOfClearedRows = 0;
            streakOfClearedRows = 0;
            CurrentLevel++;
            listOfBlocks.Clear();
            ClearMap();

            if (currentDropTimerInterval > 50)
                currentDropTimerInterval -= 50;
            else if (currentDropTimerInterval == 50)
                currentDropTimerInterval -= 49;
            else if (currentDropTimerInterval <= 1) GameOver();

            dropTimer.Interval =
                TimeSpan.FromMilliseconds(currentDropTimerInterval);

            Console.WriteLine("Interval: " + dropTimer.Interval);
        }

        /// <summary>
        /// Prida Shape do seznamu hernich objektu
        /// </summary>
        /// <param name="shape">Pridavany objekt</param>
        public void AddBlocksToList(Shape shape)
        {
            foreach (var block in shape.ListOfParts) listOfBlocks.Add(block);
        }

        /// <summary>
        /// Odstrani zadany shape ze seznamu hernich objektu
        /// </summary>
        /// <param name="shape">Odebirany objekt</param>
        public void RemoveGameObject(Shape shape)
        {
            foreach (var block in shape.ListOfParts) listOfBlocks.Remove(block);
        }

        /// <summary>
        /// Generuje nahodne cislo radku
        /// </summary>
        /// <returns>Nahodne cislo radku</returns>
        private int RandomColumn()
        {
            return random.Next(1, map.GetLength(1) - 3);
        }

        private void ClearMap()
        {
            for (var i = 0; i < map.GetLength(0); i++)
                for (var j = 0; j < map.GetLength(1); j++) map[i, j] = null;
        }

        private void AddShapeToNextShapeGrid(Shape shape)
        {
            //mainViewModel.gridNextShape.Children.Clear();
            mainViewModel.ClearNextShapeGridAction();

            if (shape != null)
                for (var row = 0; row < shape.ComposedShape.GetLength(0); row++)
                    for (var col = 0; col < shape.ComposedShape.GetLength(1); col++)
                        if (shape.ComposedShape[row, col] != null)
                        {
                            Grid.SetRow(shape.ComposedShape[row, col].Border, row);
                            Grid.SetColumn(shape.ComposedShape[row, col].Border, col);
                            mainViewModel.AddBlockToNextShapeGridAction.Invoke(shape.ComposedShape[row, col].Border);
                            //mainWindow.gridNextShape.Children.Add(shape.ComposedShape[row, col].Border);
                        }
        }

        /// <summary>
        /// Inicializace casovace, ktery zajistuje padani objektu
        /// </summary>
        private void DropTimerInit()
        {
            dropTimer = new DispatcherTimer(DispatcherPriority.Render);
            dropTimer.Interval = TimeSpan.FromMilliseconds(startingTimerInterval);
            dropTimer.Tick += DropTimer_Tick;
        }

        /// <summary>
        /// V kazdem ticku se posune objekt na danou souradnici
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropTimer_Tick(object sender, EventArgs e)
        {
            MoveShape(controlableShape, Orientation.Vertical, moveDistance);
        }

        /// <summary>
        /// Zpracovava vstup od hrace, pokud zmackne jednu z povolenych klaves,
        /// odstrani se aktualni obrazec, provede se odpovidajici akce a obrazec
        /// se opet prida do mapy, nasledne se aktualizuje herni obrazovka.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerInput(object sender, KeyEventArgs e)
        {
            if (!paused)
                if (controlableShape != null)
                    switch (e.Key)
                    {
                        case Key.Space:
                            RotateShape(controlableShape);
                            break;
                        case Key.Right:
                            MoveShape(controlableShape, Orientation.Horizontal, moveDistance);
                            break;
                        case Key.Down:
                            MoveShape(controlableShape, Orientation.Vertical, moveDistance);
                            break;
                        case Key.Left:
                            MoveShape(controlableShape, Orientation.Horizontal, -moveDistance);
                            break;
                        case Key.Escape:
                            Pause();
                            ShowMainMenu();
                            break;
                    }
        }

        /// <summary>
        /// Aktualizuje hru a herni mapu
        /// </summary>
        private void UpdateGame()
        {
            RefreshGrid();
        }

        /// <summary>
        /// Do herni mrizky priradi bordery jednotlivych bloku
        /// </summary>
        private void RefreshGrid()
        {
            //mainWindow.gameGrid.Children.Clear();
            mainViewModel.ClearGameGridAction();

            foreach (var block in listOfBlocks)
            {
                Grid.SetRow(block.Border, block.Row);
                Grid.SetColumn(block.Border, block.Column);

                //mainWindow.gameGrid.Children.Add(block.Border);
                mainViewModel.AddBlockToGameGridAction(block.Border);
            }
        }

        /// <summary>
        /// Naplni seznam vsemi tvary, ktere se mohou ve hre vysktnout
        /// </summary>
        private void AddShapeTypesToList()
        {
            listOfShapeTypes.Add(ShapeType.ShapeL);
            listOfShapeTypes.Add(ShapeType.ShapeLInverted);
            listOfShapeTypes.Add(ShapeType.ShapeI);
            listOfShapeTypes.Add(ShapeType.ShapeO);
            listOfShapeTypes.Add(ShapeType.ShapeZ);
            listOfShapeTypes.Add(ShapeType.ShapeS);
            listOfShapeTypes.Add(ShapeType.ShapeT);
        }

        /// <summary>
        /// Vygeneruje soucasny a nasledujici objekt a prida je do odpovidajicich
        /// gridu
        /// </summary>
        private void SpawnShapes()
        {
            //mainWindow.gridNextShape.Children.Clear();
            mainViewModel.ClearNextShapeGridAction();
            controlableShape = nextShape;
            AddShapeToGame(controlableShape);
            nextShape = SpawnNewShape(row, column);
            AddShapeToNextShapeGrid(nextShape);
        }

        /// <summary>
        /// Prida do hry novy herni objekt
        /// </summary>
        private Shape SpawnNewShape(int row, int column)
        {
            Console.WriteLine("X: {0} : Y: {1}", column, row);
            Console.WriteLine("New shape spawned");

            // vytvori nahodny objekt
            var newShape = GetRandomShape(row, column);
            ChooseRandomRotation(newShape);

            return newShape;
        }

        /// <summary>
        /// Nekolikrat (nahodne) zrotuje objekt.
        /// </summary>
        /// <param name="shape"></param>
        private void ChooseRandomRotation(Shape shape)
        {
            if (shape != null)
            {
                var range = random.Next(10);
                // na zaklade nahodneho cisla zrotuje objekt
                for (var i = 0; i < range; i++) shape.Rotate();
            }
        }


        private void AddShapeToGame(Shape shape)
        {
            if (!(shape is null))
                AddBlocksToList(shape);
            UpdateGame();
        }

        private Shape GetConcreteShape(ShapeType type, int row, int column)
        {
            Shape newShape = null;

            switch (type)
            {
                case ShapeType.ShapeL:
                    newShape = new ShapeL(row, column);
                    break;
                case ShapeType.ShapeLInverted:
                    newShape = new ShapeLInverted(row, column);
                    break;
                case ShapeType.ShapeI:
                    newShape = new ShapeI(row, column);
                    break;
                case ShapeType.ShapeO:
                    newShape = new ShapeO(row, column);
                    break;
                case ShapeType.ShapeZ:
                    newShape = new ShapeZ(row, column);
                    break;
                case ShapeType.ShapeS:
                    newShape = new ShapeS(row, column);
                    break;
                case ShapeType.ShapeT:
                    newShape = new ShapeT(row, column);
                    break;
            }

            return newShape;
        }

        private Shape GetRandomShape(int row, int column)
        {
            var randomIndex = random.Next(listOfShapeTypes.Count);
            var randomShapeType = listOfShapeTypes[randomIndex];

            return GetConcreteShape(randomShapeType, row, column);
        }

        private void AddBlockToMap(Block block)
        {
            map[block.Row, block.Column] = block;
        }


        private void RemoveBlockFromMap(Block block)
        {
            map[block.Row, block.Column] = null;
        }

        private void RemoveBlockFromGrid(Block block)
        {
            //mainWindow.gameGrid.Children.Remove(block.Border);
            mainViewModel.ClearGameGridAction();
        }

        private void RemoveBlock(Block block)
        {
            RemoveBlockFromMap(block);
            RemoveBlockFromGrid(block);
        }

        /// <summary>
        /// Vezme predany objekt, a na odpovidajici pozice na mape vlozi jeho 
        /// jednotlive casti
        /// </summary>
        /// <param name="shape">Objekt pro pridani do mapy</param>
        private void AddShapeToMap(Shape shape)
        {
            foreach (var part in shape.ListOfParts) map[part.Row, part.Column] = part;
        }

        /// <summary>
        /// Odstrani obrazec z herni mapy, tak ze jednotlive souradnice nastavi 
        /// na NULL
        /// </summary>
        /// <param name="shape">Obrazec pro odstraneni</param>
        private void RemoveShapeFromMap(Shape shape)
        {
            foreach (var part in shape.ListOfParts) map[part.Row, part.Column] = null;
        }

        /// <summary>
        /// Odstrani dany obrazec z herni obrazovky
        /// </summary>
        /// <param name="shape">Obrazec pro odstraneni</param>
        private void RemoveShapeFromGrid(Shape shape)
        {
            foreach (var part in shape.ListOfParts)
            {
                //mainWindow.gameGrid.Children.Remove(part.Border);
                mainViewModel.RemoveBlockFromGameGridAction(part.Border);
            }
        }

        /// <summary>
        /// Odstrani zadany obrazec z herni mapy a herni obrazovky
        /// </summary>
        /// <param name="shape">Obrazec pro odstraneni</param>
        private void RemoveShape(Shape shape)
        {
            RemoveShapeFromMap(shape);
            RemoveShapeFromGrid(shape);
        }

        /// <summary>
        /// Zkotroluje, zda nedoslo ke kolizi, pokud ne, 
        /// odstrani puvodni objekt z mapy, zmeni jeho pozici a opet ho do mapy 
        /// prida.
        /// </summary>
        /// <param name="shape">Objekt pro posun</param>
        /// <param name="orientation">Orientace - vertikalni/horizontalni</param>
        /// <param name="distance">Vzdalenost posunu</param>
        private void MoveShape(Shape shape, Orientation orientation, int distance)
        {
            if (shape != null)
            {
                CheckMovementCollision(shape, orientation, distance);

                if (!collisionDetected)
                {
                    RemoveShape(shape);
                    shape.Move(orientation, distance);
                    AddShapeToMap(shape);
                    UpdateGame();
                }
                Console.WriteLine("MAX ROW: " + (map.GetLength(0) - 1));
                Console.WriteLine("ROW: " + shape.Row + " Column: " + shape.Column);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        private void CheckMovementCollision(Shape shape, Orientation orientation, int distance)
        {
            collisionDetected = false;

            foreach (var part in shape.ListOfParts)
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        if (part.Column + distance < 0
                            || part.Column + distance
                            > map.GetLength(1) - 1)
                        {
                            collisionDetected = true;
                            break;
                        }
                        else if (map[part.Row, part.Column + distance] != null)
                        {
                            if (map[part.Row, part.Column + distance] != shape.Part1
                                && map[part.Row, part.Column + distance] != shape.Part2
                                && map[part.Row, part.Column + distance] != shape.Part3
                                && map[part.Row, part.Column + distance] != shape.Part4)
                            {
                                collisionDetected = true;

                                break;
                            }
                        }
                        break;
                    case Orientation.Vertical:
                        if (part.Row + distance < 0)
                        {
                            collisionDetected = true;
                            break;
                        }
                        // kolize se spodnim krajem obrazovky
                        else if (
                            part.Row + distance > map.GetLength(0) - 1)
                        {
                            collisionDetected = true;
                            CheckRows();
                            CalculateScore();
                            SpawnShapes();
                            return;
                        }
                        else if
                            (map[part.Row + distance, part.Column] != null)
                        {
                            if (map[part.Row + distance, part.Column] != shape.Part1
                                && map[part.Row + distance, part.Column] != shape.Part2
                                && map[part.Row + distance, part.Column] != shape.Part3
                                && map[part.Row + distance, part.Column] != shape.Part4)
                            {
                                collisionDetected = true;

                                CheckRows();
                                CalculateScore();
                                SpawnShapes();

                                return;
                            }
                        }
                        break;
                }
        }

        /// <summary>
        /// Zkontroluje, zda po rotaci nezasahuje objekt mimo herni obrazovku
        /// </summary>
        /// <param name="shape"></param>
        private void CheckRotationCollision(Shape shape)
        {
            collisionDetected = false;

            foreach (var part in shape.ListOfParts)
                if (part.Row < 0
                    || part.Row > map.GetLength(0) - 1)
                {
                    collisionDetected = true;
                    break;
                }
                else if (part.Column < 0)
                {
                    collisionDetected = true;
                }
                else if (part.Column >= map.GetLength(1) - 1)
                {
                    collisionDetected = true;
                }
                else if (map[part.Row, part.Column] != null)
                {
                    if (map[part.Row, part.Column] != shape.Part1
                        && map[part.Row, part.Column] != shape.Part2
                        && map[part.Row, part.Column] != shape.Part3
                        && map[part.Row, part.Column] != shape.Part4)
                    {
                        collisionDetected = true;

                        break;
                    }
                }
        }

        /// <summary>
        /// Odstrani zadany objekt z mapy, otoci ho, zkontroluje, zda nedochazi
        /// ke kolizi a opet ho prida do mapy, nakonec aktualizuje hru
        /// </summary>
        /// <param name="shape"></param>
        private void RotateShape(Shape shape)
        {
            if (shape != null)
            {
                var previousRotation = shape.RotationState;

                RemoveShape(shape);

                shape.Rotate();
                CheckRotationCollision(shape);

                // pokud objekt zasahuje mimo herni pole, vrati se puvodni stav
                if (collisionDetected) shape.ChangeRotationState(previousRotation);

                AddShapeToMap(shape);
                UpdateGame();
            }
        }

        private void CheckRows()
        {
            CheckTopRow();
            CheckFullRows();

            if (totalNumberOfClearedRows >= clearedRowsLimit) LevelUp();
        }

        /// <summary>
        /// Zkontroluje jestli nektery block na herni mape nezasahuje na prvni 
        /// radek mimo herni obrazovku, pokud ano, vyvola konec hry
        /// </summary>
        private void CheckTopRow()
        {
            var row = 5;

            for (var col = 0; col < map.GetLength(1); col++)
                if (map[row, col] != null)
                {
                    Console.WriteLine("Ending game: ROW: {0} COL: {1}", row, col);
                    GameOver();
                }
        }

        /// <summary>
        /// Zkontroluje, zda je radek zaplnen
        /// </summary>
        private void CheckFullRows()
        {
            var isRowFull = true;
            for (var row = map.GetLength(0) - 1; row >= 0; row--)
            {
                isRowFull = true;
                for (var column = 0; column < map.GetLength(1); column++)
                    if (map[row, column] == null)
                    {
                        isRowFull = false;
                        break;
                    }

                // radek je plny
                if (isRowFull)
                {
                    DeleteRow(row);
                    DropRows(row);
                    CheckRows();
                }
            }
            Console.WriteLine("Row: {0} is full: {1}", row, isRowFull);
        }

        /// <summary>
        /// Vymaze zadany radek
        /// </summary>
        /// <param name="rowNumber"></param>
        private void DeleteRow(int rowNumber)
        {
            for (var column = 0; column < map.GetLength(1); column++)
            {
                listOfBlocks.Remove(map[rowNumber, column]);
                map[rowNumber, column] = null;
            }

            streakOfClearedRows++;
            totalNumberOfClearedRows++;
        }

        /// <summary>
        /// Vymaze radky z mapy, az do zadaneho radku
        /// </summary>
        /// <param name="rowNumber"></param>
        private void DeleteRowsUntil(int rowNumber)
        {
            for (var row = 0; row < rowNumber; row++)
                for (var col = 0; col < map.GetLength(1); col++) map[row, col] = null;
        }

        /// <summary>
        /// Posune smerem dolu vsechny radky nad smazanym
        /// </summary>
        private void DropRows(int rowNumber)
        {
            ClearMap();

            foreach (var block in listOfBlocks)
            {
                if (block.Row < rowNumber) block.Row++;
                AddBlockToMap(block);
            }
        }

        /// <summary>
        /// Vypocitava skore, na zaklade najednou odstranenych radku
        /// </summary>
        /// <param name="numberOfDroppedRows"></param>
        private void CalculateScore()
        {
            var value = 0;

            switch (streakOfClearedRows)
            {
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
            CurrentScore += value * currentLevel;
            streakOfClearedRows = 0;
        }

        private void GameOver()
        {
            CreateRecord();
            Pause();
            ShowNewGameDialog();
        }

        /// <summary>
        /// Zepta se hrace, jestli chce zacit novou hru
        /// </summary>
        private void ShowNewGameDialog()
        {
            var result = ShowDialog(
                "Konec hry, přejete si začít znovu?", "Konec hry");

            switch (result)
            {
                case MessageBoxResult.Yes:
                    StartNewGame();
                    return;
                case MessageBoxResult.No:
                    ShowEndGameDialog();
                    break;
            }
        }

        /// <summary>
        /// Zepta se hrace, jestli chce hru vypnout
        /// </summary>
        private void ShowEndGameDialog()
        {
            var result = ShowDialog(
                        "Přejete si hru vypnout?", "Konec hry");

            switch (result)
            {
                case MessageBoxResult.Yes:
                    Exit();
                    break;
                case MessageBoxResult.No:
                    GameOver();
                    return;
            }
        }

        private MessageBoxResult ShowDialog(string msg, string title)
        {
            return MessageBox.Show(msg, title, MessageBoxButton.YesNo);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
