﻿using Tetris.Logic;
using Tetris.Logic.Enums;

namespace Tetris.Models.Shapes
{
    public sealed class ShapeLInverted : Shape
    {

        #region Constructor

        public ShapeLInverted(Position position) : base(position)
        {
            Initialize();
        }

        #endregion Constructor

        #region Overriden Methods

        protected override void Initialize()
        {
            Type = ShapeType.ShapeLInverted;
            NumberOfRotationStates = 4;
            ChangeRotationToDefault();
        }

        protected override void ChangeRotationToDefault()
        {
            RotationState = RotationState.Default;

            ComposedShape = new[,]
                {
                    { null,       Part1,       null,       null },
                    { null,       Part2,       null,       null },
                    { Part3,      Part4,       null,       null },
                    { null,       null,        null,       null }
                };

            Part1.Position = Position;
            Part2.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column);
            Part3.Position = new Position(Part1.Position.Row + 2, Part1.Position.Column - 1);
            Part4.Position = new Position(Part1.Position.Row + 2, Part1.Position.Column);
        }

        protected override void ChangeRotationToRotatedOnce()
        {
            RotationState = RotationState.RotatedOnce;

            ComposedShape = new[,]
                {
                    { Part1,       null,       null,       null },
                    { Part2,       Part3,      Part4,      null },
                    { null,        null,       null,       null },
                    { null,        null,       null,       null }
                };

            Part1.Position = new Position(Position.Row + 1, Position.Column - 1);
            Part2.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column);
            Part3.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column + 1);
            Part4.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column + 2);
        }

        protected override void ChangeRotationToRotatedTwice()
        {
            RotationState = RotationState.RotatedTwice;

            ComposedShape = new[,]
                {
                    { null,       Part1,       Part2,       null },
                    { null,       Part3,       null,        null },
                    { null,       Part4,       null,        null },
                    { null,       null,        null,        null }
                };

            Part1.Position = new Position(Position.Row, Position.Column - 1);
            Part2.Position = new Position(Part1.Position.Row, Part1.Position.Column + 1);
            Part3.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column);
            Part4.Position = new Position(Part1.Position.Row + 2, Part1.Position.Column);
        }

        protected override void ChangeRotationToRotatedThreeTimes()
        {
            RotationState = RotationState.RotatedThreeTimes;

            ComposedShape = new[,]
                {
                    { Part1,      Part2,      Part3,      null },
                    { null,       null,       Part4,      null },
                    { null,       null,       null,       null },
                    { null,       null,       null,       null }
                };

            Part1.Position = new Position(Position.Row, Position.Column - 1);
            Part2.Position = new Position(Part1.Position.Row, Part1.Position.Column + 1);
            Part3.Position = new Position(Part1.Position.Row, Part1.Position.Column + 2);
            Part4.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column + 2);
        }

        #endregion Overriden Methods
    }
}
