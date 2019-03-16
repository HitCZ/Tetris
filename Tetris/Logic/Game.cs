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
using Tetris.Logic.Providers;
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
        private int startingRow = 2;
        private int startingColumn = 4;
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

        /// <summary/>
        public Game(MainViewModel mainViewModel, int numberOfRows, int numberOfColumns, string playerName)
        {
            this.mainViewModel = mainViewModel;
            this.numberOfRows = numberOfRows;
            this.numberOfColumns = numberOfColumns;
            this.playerName = playerName;

            Initialize();
            InitializeCommands();
        }

        #endregion Constructor

        #region Public Methods


        /// <summary>
        /// Starts new game
        /// </summary>
        public void StartNewGame()
        {
            Reset();
            mainViewModel.RemoveBlur();
            controlableShape = SpawnNewShape(new Position(startingRow, startingColumn));
            AddShapeToGame(controlableShape);

            nextShape = SpawnNewShape(new Position(startingRow, startingColumn));
            AddShapeToNextShapeGrid(nextShape);

            dropTimer.Start();
        }

        /// <summary>
        /// Pauses the game and displays the Pause Menu
        /// </summary>
        public void Pause()
        {
            paused = true;
            dropTimer.Stop();
            mainViewModel.AddBlur();
        }

        /// <summary>
        /// Resumes the game
        /// </summary>
        public void Resume()
        {
            paused = false;
            mainViewModel.RemoveBlur();
            dropTimer.Start();
        }

        /// <summary>
        /// Terminates the application
        /// </summary>
        public void Exit() => Application.Current.Shutdown();

        /// <summary>
        /// Creates a record in the XML file
        /// </summary>
        public void CreateRecord()
        {
            var r = new Record(playerName, currentCurrentScore, totalNumberOfClearedRows, currentLevel);

            if (writer is null)
                writer = new FileWriter();

            writer.Write(r);
        }

        /// <summary>
        /// Adds the given shape to the list of game objects
        /// </summary>
        public void AddBlocksToList(Shape shape)
        {
            foreach (var block in shape.ListOfParts)
            {
                listOfBlocks.Add(block);
            }
        }

        #endregion Public Methods

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

        private void Initialize()
        {
            random = new Random();
            listOfShapeTypes = new List<ShapeType>();
            listOfBlocks = new List<Block>();
            currentDropTimerInterval = startingTimerInterval;
            map = new Block[numberOfRows, numberOfColumns];

            DropTimerInit();
        }

        private void ShowMainMenu()
        {
            var pauseMenu = new PauseMenuView(this);
            pauseMenu.Show();
        }

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

            dropTimer.Interval = TimeSpan.FromMilliseconds(currentDropTimerInterval);
        }

        private void ClearMap()
        {
            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = null;
                }
            }
        }

        private void AddShapeToNextShapeGrid(Shape shape)
        {
            mainViewModel.ClearNextShapeGridAction();

            if (shape is null)
                return;

            for (var rowIndex = 0; rowIndex < shape.ComposedShape.GetLength(0); rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < shape.ComposedShape.GetLength(1); columnIndex++)
                {
                    if (shape.ComposedShape[rowIndex, columnIndex] is null)
                        continue;

                    Grid.SetRow(shape.ComposedShape[rowIndex, columnIndex].Border, rowIndex);
                    Grid.SetColumn(shape.ComposedShape[rowIndex, columnIndex].Border, columnIndex);
                    mainViewModel.AddBlockToNextShapeGridAction.Invoke(shape.ComposedShape[rowIndex, columnIndex].Border);
                }
            }
        }

        private void DropTimerInit()
        {
            dropTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(startingTimerInterval)
            };
            dropTimer.Tick += DropTimer_Tick;
        }

        private void DropTimer_Tick(object sender, EventArgs e)
        {
            MoveShape(controlableShape, Orientation.Vertical, moveDistance);
        }

        private void UpdateGame() => RefreshGrid();

        private void RefreshGrid()
        {
            mainViewModel.ClearGameGridAction();

            foreach (var block in listOfBlocks)
            {
                Grid.SetRow(block.Border, block.Position.Row);
                Grid.SetColumn(block.Border, block.Position.Column);

                mainViewModel.AddBlockToGameGridAction(block.Border);
            }
        }

        private void SpawnShapes()
        {
            mainViewModel.ClearNextShapeGridAction();
            controlableShape = nextShape;
            AddShapeToGame(controlableShape);
            nextShape = SpawnNewShape(new Position(startingRow, startingColumn));
            AddShapeToNextShapeGrid(nextShape);
        }

        // ReSharper disable once ParameterHidesMember
        // ReSharper disable once ParameterHidesMember
        private Shape SpawnNewShape(Position position)
        {
            var newShape = ShapeProvider.GetRandomShape(position);
            RandomlyRotateShape(newShape);

            return newShape;
        }

        private void RandomlyRotateShape(Shape shape)
        {
            if (shape is null)
                return;

            var range = random.Next(10);

            for (var i = 0; i < range; i++)
            {
                shape.Rotate();
            }
        }

        private void AddShapeToGame(Shape shape)
        {
            if (!(shape is null))
                AddBlocksToList(shape);
            UpdateGame();
        }

        private void AddBlockToMap(Block block)
        {
            map[block.Position.Row, block.Position.Column] = block;
        }

        private void RemoveBlockFromMap(Block block)
        {
            map[block.Position.Row, block.Position.Column] = null;
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
            foreach (var part in shape.ListOfParts)
            {
                map[part.Position.Row, part.Position.Column] = part;
            }
        }

        /// <summary>
        /// Odstrani obrazec z herni mapy, tak ze jednotlive souradnice nastavi 
        /// na NULL
        /// </summary>
        /// <param name="shape">Obrazec pro odstraneni</param>
        private void RemoveShapeFromMap(Shape shape)
        {
            foreach (var part in shape.ListOfParts)
            {
                map[part.Position.Row, part.Position.Column] = null;
            }
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

        private void MoveShape(Shape shape, Orientation orientation, int distance)
        {
            if (shape == null)
                return;
            CheckMovementCollision(shape, orientation, distance);

            if (collisionDetected)
                return;

            RemoveShape(shape);
            shape.Move(orientation, distance);
            AddShapeToMap(shape);
            UpdateGame();
        }

        private void CheckMovementCollision(Shape shape, Orientation orientation, int distance)
        {
            collisionDetected = false;

            foreach (var part in shape.ListOfParts)
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        if (part.Position.Column + distance < 0 || part.Position.Column + distance > map.GetLength(1) - 1)
                        {
                            collisionDetected = true;
                        }
                        else if (map[part.Position.Row, part.Position.Column + distance] != null)
                        {
                            if (map[part.Position.Row, part.Position.Column + distance] != shape.Part1
                                && map[part.Position.Row, part.Position.Column + distance] != shape.Part2
                                && map[part.Position.Row, part.Position.Column + distance] != shape.Part3
                                && map[part.Position.Row, part.Position.Column + distance] != shape.Part4)
                            {
                                collisionDetected = true;
                            }
                        }
                        break;
                    case Orientation.Vertical:
                        if (part.Position.Row + distance < 0)
                        {
                            collisionDetected = true;
                        }
                        else if (
                            part.Position.Row + distance > map.GetLength(0) - 1)
                        {
                            collisionDetected = true;
                            CheckRows();
                            CalculateScore();
                            SpawnShapes();
                            return;
                        }
                        else if
                            (map[part.Position.Row + distance, part.Position.Column] != null)
                        {
                            if (   map[part.Position.Row + distance, part.Position.Column] != shape.Part1
                                && map[part.Position.Row + distance, part.Position.Column] != shape.Part2
                                && map[part.Position.Row + distance, part.Position.Column] != shape.Part3
                                && map[part.Position.Row + distance, part.Position.Column] != shape.Part4)
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

        private void CheckRotationCollision(Shape shape)
        {
            collisionDetected = false;

            foreach (var part in shape.ListOfParts)
                if (part.Position.Row < 0 || part.Position.Row > map.GetLength(0) - 1)
                {
                    collisionDetected = true;
                    break;
                }
                else if (part.Position.Column < 0)
                {
                    collisionDetected = true;
                }
                else if (part.Position.Column >= map.GetLength(1) - 1)
                {
                    collisionDetected = true;
                }
                else if (map[part.Position.Row, part.Position.Column] != null)
                {
                    if (map[part.Position.Row, part.Position.Column] == shape.Part1 || 
                        map[part.Position.Row, part.Position.Column] == shape.Part2 ||
                        map[part.Position.Row, part.Position.Column] == shape.Part3 ||
                        map[part.Position.Row, part.Position.Column] == shape.Part4)
                        continue;

                    collisionDetected = true;

                    break;
                }
        }

        private void RotateShape(Shape shape)
        {
            if (shape != null)
            {
                var previousRotation = shape.RotationState;

                var previousPartsPositions = new[]
                {
                    shape.Part1.Position,
                    shape.Part2.Position,
                    shape.Part3.Position,
                    shape.Part4.Position
                };

                RemoveShape(shape);

                shape.Rotate();
                CheckRotationCollision(shape);

                // pokud objekt zasahuje mimo herni pole, vrati se puvodni stav
                if (collisionDetected)
                {
                    shape.ChangeRotationState(previousRotation);
                    shape.Part1.Position = previousPartsPositions[0];
                    shape.Part2.Position = previousPartsPositions[1];
                    shape.Part3.Position = previousPartsPositions[2];
                    shape.Part4.Position = previousPartsPositions[3];
                }

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
            Console.WriteLine("Row: {0} is full: {1}", startingRow, isRowFull);
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
                if (block.Position.Row < rowNumber)
                {
                    var blockPosition = block.Position;
                    blockPosition.Row++;
                    block.Position = blockPosition;
                }
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

        #endregion Private Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}
