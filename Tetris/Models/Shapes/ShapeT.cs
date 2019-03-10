﻿using Tetris.Logic.Enums;

namespace Tetris.Models.Shapes
{
    public sealed class ShapeT : Shape
    {
        #region Constructor

        public ShapeT(int row, int column) : base(row, column)
        {
            Initialize();
        }

        #endregion Constructor

        #region Overriden Methods

        protected override void Initialize()
        {
            Type = ShapeType.ShapeT;
            NumberOfRotationStates = 4;
            ChangeRotationToDefault();
        }

        protected override void ChangeRotationToDefault()
        {
            RotationState = RotationState.Default;

            ComposedShape = new[,]
            {
                { null,       Part1,      null,       null },
                { Part2,      Part3,      Part4,      null },
                { null,       null,       null,       null },
                { null,       null,       null,       null }
            };

            Part1.Row = Row;
            Part1.Column = Column;

            Part2.Row = Part1.Row + 1;
            Part2.Column = Part1.Column - 1;

            Part3.Row = Part1.Row + 1;
            Part3.Column = Part1.Column;

            Part4.Row = Part1.Row + 1;
            Part4.Column = Part1.Column + 1;
        }

        protected override void ChangeRotationToRotatedOnce()
        {
            RotationState = RotationState.RotatedOnce;

            ComposedShape = new[,]
            {
                { Part1,      null,       null,      null },
                { Part2,      Part3,      null,      null },
                { Part4,      null,       null,      null },
                { null,       null,       null,      null }

            };

            Part1.Row = Row - 1;
            Part1.Column = Column - 1;

            Part2.Row = Part1.Row + 1;
            Part2.Column = Part1.Column;

            Part3.Row = Part1.Row + 1;
            Part3.Column = Part1.Column + 1;

            Part4.Row = Part1.Row + 2;
            Part4.Column = Part1.Column;
        }

        protected override void ChangeRotationToRotatedTwice()
        {
            RotationState = RotationState.RotatedTwice;

            ComposedShape = new[,]
            {
                { Part1,      Part2,      Part3,      null },
                { null,       Part4,      null,       null },
                { null,       null,       null,       null },
                { null,       null,       null,       null }
            };

            Part1.Row = Row - 1;
            Part1.Column = Column - 1;

            Part2.Row = Part1.Row;
            Part2.Column = Part1.Column + 1;

            Part3.Row = Part1.Row;
            Part3.Column = Part1.Column + 2;

            Part4.Row = Part1.Row + 1;
            Part4.Column = Part1.Column + 1;
        }

        protected override void ChangeRotationToRotatedThreeTimes()
        {
            RotationState = RotationState.RotatedThreeTimes;

            ComposedShape = new[,]
            {
                { null,       Part1,      null,       null },
                { Part2,      Part3,      null,       null },
                { null,       Part4,      null,       null },
                { null,       null,       null,       null }
            };

            Part1.Row = Row - 1;
            Part1.Column = Column + 1;

            Part2.Row = Part1.Row + 1;
            Part2.Column = Part1.Column - 1;

            Part3.Row = Part1.Row + 1;
            Part3.Column = Part1.Column;

            Part4.Row = Part1.Row + 2;
            Part4.Column = Part1.Column;
        }

        #endregion Overriden Methods
    }
}
