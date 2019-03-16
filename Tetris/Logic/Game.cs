using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Tetris.Annotations;
using Tetris.Logic.Providers;
using Tetris.Models;
using Tetris.Models.Shapes;
using Tetris.Properties;
using Tetris.ViewModels;
using Tetris.Views;
using Orientation = Tetris.Logic.Enums.Orientation;

namespace Tetris.Logic
{
    public class Game : INotifyPropertyChanged
    {
        #region Fields

        private readonly MainViewModel mainViewModel;
        private readonly string playerName;
        private readonly int numberOfRows;
        private readonly int numberOfColumns;
        private int startingRow = 2;
        private int startingColumn = 4;
        private int currentLevel = 1;
        private int streakOfClearedRows;
        private int totalNumberOfClearedRows;
        private int clearedRowsLimit = 15;
        private int startingTimerInterval = 750;
        private int currentDropTimerInterval;
        private int currentCurrentScore;
        private int moveDistance = 1;
        private bool paused;
        private FileWriter writer;
        private Block[,] map;
        private Random random;
        private Shape controlableShape;
        private Shape nextShape;
        private List<Block> listOfBlocks;
        private DispatcherTimer dropTimer;

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

        private void AddShapeToMap(Shape shape)
        {
            foreach (var part in shape.ListOfParts)
            {
                map[part.Position.Row, part.Position.Column] = part;
            }
        }

        private void RemoveShapeFromMap(Shape shape)
        {
            foreach (var part in shape.ListOfParts)
            {
                map[part.Position.Row, part.Position.Column] = null;
            }
        }

        private void RemoveShapeFromGrid(Shape shape)
        {
            foreach (var part in shape.ListOfParts)
            {
                mainViewModel.RemoveBlockFromGameGridAction(part.Border);
            }
        }

        private void RemoveShape(Shape shape)
        {
            RemoveShapeFromMap(shape);
            RemoveShapeFromGrid(shape);
        }

        private void MoveShape(Shape shape, Orientation orientation, int distance)
        {
            if (shape is null)
                return;

            var collisionDetected = CheckMovementCollision(shape, orientation, distance);

            if (collisionDetected)
                return;

            RemoveShape(shape);
            shape.Move(orientation, distance);
            AddShapeToMap(shape);
            UpdateGame();
        }

        private bool CheckMovementCollision(Shape shape, Orientation orientation, int distance)
        {
            foreach (var part in shape.ListOfParts)
            {
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        if (part.Position.Column + distance < 0 ||
                            part.Position.Column + distance > map.GetLength(1) - 1)
                        {
                            return true;
                        }
                        else if (map[part.Position.Row, part.Position.Column + distance] != null)
                        {
                            if (map[part.Position.Row, part.Position.Column + distance] != shape.Part1
                                && map[part.Position.Row, part.Position.Column + distance] != shape.Part2
                                && map[part.Position.Row, part.Position.Column + distance] != shape.Part3
                                && map[part.Position.Row, part.Position.Column + distance] != shape.Part4)
                            {
                                return true;
                            }
                        }

                        break;
                    case Orientation.Vertical:
                        if (part.Position.Row + distance < 0)
                        {
                            return true;
                        }
                        else if (
                            part.Position.Row + distance > map.GetLength(0) - 1)
                        {
                            CheckRows();
                            CalculateScore();
                            SpawnShapes();
                            return true;
                        }
                        else if
                            (map[part.Position.Row + distance, part.Position.Column] != null)
                        {
                            if (map[part.Position.Row + distance, part.Position.Column] != shape.Part1
                                && map[part.Position.Row + distance, part.Position.Column] != shape.Part2
                                && map[part.Position.Row + distance, part.Position.Column] != shape.Part3
                                && map[part.Position.Row + distance, part.Position.Column] != shape.Part4)
                            {
                                CheckRows();
                                CalculateScore();
                                SpawnShapes();

                                return true;
                            }
                        }

                        break;
                }
            }

            return false;
        }

        private bool CheckRotationCollision(Shape shape)
        {
            foreach (var part in shape.ListOfParts)
                if (part.Position.Row < 0 || part.Position.Row > map.GetLength(0) - 1)
                {
                    return true;
                }
                else if (part.Position.Column < 0)
                {
                    return true;
                }
                else if (part.Position.Column >= map.GetLength(1) - 1)
                {
                    return true;
                }
                else if (map[part.Position.Row, part.Position.Column] != null)
                {
                    if (map[part.Position.Row, part.Position.Column] == shape.Part1 ||
                        map[part.Position.Row, part.Position.Column] == shape.Part2 ||
                        map[part.Position.Row, part.Position.Column] == shape.Part3 ||
                        map[part.Position.Row, part.Position.Column] == shape.Part4)
                        continue;

                    return true;
                }

            return false;
        }

        private void RotateShape(Shape shape)
        {
            if (shape is null)
                return;

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
            var collisionDetected = CheckRotationCollision(shape);

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

        private void CheckRows()
        {
            CheckTopRow();
            CheckFullRows();

            if (totalNumberOfClearedRows >= clearedRowsLimit)
                LevelUp();
        }

        private void CheckTopRow()
        {
            const int row = 5;

            for (var col = 0; col < map.GetLength(1); col++)
            {
                if (map[row, col] != null)
                    GameOver();
            }
        }

        private void CheckFullRows()
        {
            for (var row = map.GetLength(0) - 1; row >= 0; row--)
            {
                var isRowFull = true;
                for (var column = 0; column < map.GetLength(1); column++)
                    if (map[row, column] is null)
                    {
                        isRowFull = false;
                        break;
                    }

                if (!isRowFull)
                    continue;

                DeleteRow(row);
                DropRows(row);
                CheckRows();
            }
        }

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

        private void ShowNewGameDialog()
        {
            var result = ShowDialog(Strings.MSG_ENDGAME, Strings.TITLE_ENDGAME);

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

        private void ShowEndGameDialog()
        {
            var result = ShowDialog(Strings.MSG_QUITGAME, Strings.MSG_ENDGAME);

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

        private MessageBoxResult ShowDialog(string msg, string title) => MessageBox.Show(msg, title, MessageBoxButton.YesNo);

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
