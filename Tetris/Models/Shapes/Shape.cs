﻿using System;
using System.Collections.Generic;
using System.Windows.Media;
using Tetris.Logic;
using Tetris.Logic.Enums;
using Tetris.Logic.Providers;

namespace Tetris.Models.Shapes
{
    /// <summary>
    /// Represents template for Tetris shapes.
    /// </summary>
    public abstract class Shape
    {
        #region Fields

        private Position position;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes reference to the main view and sets random color.
        /// </summary>
        protected Shape(Position position)
        {
            Position = position;

            ListOfParts = new List<Block>();
            Color = ColorProvider.GetRandomBrush();

            CreateParts(Color);

            ComposedShape = new Block[4, 4];
        }

        #endregion Constructors

        #region Abstract Methods

        protected abstract void Initialize();
        protected abstract void ChangeRotationToDefault();
        protected abstract void ChangeRotationToRotatedOnce();
        protected abstract void ChangeRotationToRotatedTwice();
        protected abstract void ChangeRotationToRotatedThreeTimes();

        #endregion Abstract Methods

        #region Properties

        /// <summary>
        /// Gets color of the shape
        /// </summary>
        public Brush Color { get; }

        /// <summary>
        /// Gets or sets the array of assembled shape.
        /// </summary>
        public Block[,] ComposedShape { get; protected set; }

        /// <summary>
        /// Gets or sets part of the shape.
        /// </summary>
        public Block Part1
        {
            get => ListOfParts[0];
            set => ListOfParts[0] = value;
        }

        /// <summary>
        /// Gets or sets part of the shape.
        /// </summary>
        public Block Part2
        {
            get => ListOfParts[1];
            set => ListOfParts[1] = value;
        }

        /// <summary>
        /// Gets or sets part of the shape.
        /// </summary>
        public Block Part3
        {
            get => ListOfParts[2];
            set => ListOfParts[2] = value;
        }

        /// <summary>
        /// Gets or sets part of the shape.
        /// </summary>
        public Block Part4
        {
            get => ListOfParts[3];
            set => ListOfParts[3] = value;
        }

        /// <summary>
        /// Gets or sets current rotation state.
        /// </summary>
        public RotationState RotationState { get; set; }

        public ShapeType Type { get; protected set; }

        /// <summary>
        /// Gets parts of the shape.
        /// </summary>
        public List<Block> ListOfParts { get; }

        public Position Position {
            get => position;
            set => position = value;
        }

        /// <summary>
        /// Gets or sets the number of possible rotations.
        /// </summary>
        public int NumberOfRotationStates { get; set; }

        #endregion Properties

        #region Public Methods
        
        /// <summary>
        /// Rotates the shape.
        /// </summary>
        public virtual void Rotate()
        {
            if (NumberOfRotationStates == 0)
                throw new InvalidOperationException("Shape doesn't have a set number of rotation states");

            ChangeRotationState(RotationState);
        }

        /// <summary>
        /// Rotates object to the given state.
        /// </summary>
        public void ChangeRotationState(RotationState state)
        {
            switch (state)
            {
                case RotationState.Default:
                    ChangeRotationToRotatedOnce();
                    break;
                case RotationState.RotatedOnce:
                    ChangeRotationToRotatedTwice();
                    break;
                case RotationState.RotatedTwice:
                    ChangeRotationToRotatedThreeTimes();
                    break;
                case RotationState.RotatedThreeTimes:
                    ChangeRotationToDefault();
                    break;
            }
        }

        /// <summary>
        /// Moves object in given direction by given distance.
        /// </summary>
        public void Move(Orientation orientation, int distance)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    MoveHorizontally(distance);
                    break;
                case Orientation.Vertical:
                    MoveVertically(distance);
                    break;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void MoveHorizontally(int distance)
        {
            position.Column += distance;

            foreach (var part in ListOfParts)
            {
                var partPosition = part.Position;
                partPosition.Column += distance;

                part.Position = partPosition;
            }
        }

        private void MoveVertically(int distance)
        {
            position.Row += distance;

            foreach (var part in ListOfParts)
            {
                var partPosition = part.Position;
                partPosition.Row += distance;

                part.Position = partPosition;
            }
        }

        private void CreateParts(Brush color)
        {
            for (var i = 0; i < 4; i++)
            {
                ListOfParts?.Add(new Block(color));
            }
        }

        #endregion Private Methods
    }
}
