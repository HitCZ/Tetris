﻿using Tetris.Logic.Enums;

namespace Tetris.Logic.ShapeClasses
{
    class ShapeZ : Shape
    {

        #region Constructor

        public ShapeZ(int row, int column, Game game) : base(row, column, game)
        {
        }

        #endregion Constructor

        #region Overriden Methods

        protected override void Initialize()
        {
            Type = ShapeType.ShapeZ;
            NumberOfRotationStates = 2;
            ChangeRotationToDefault();
        }

        protected override void ChangeRotationToDefault()
        {
            RotationState = RotationState.Default;
            ComposedShape = new[,]
                    {
                        { Part1,      Part2,      null,       null },
                        { null,       Part3,      Part4,      null },
                        { null,       null,       null,       null },
                        { null,       null,       null,       null }
                    };

            Part1.Row = Row;
            Part1.Column = Column;

            Part2.Row = Part1.Row;
            Part2.Column = Part1.Column + 1;

            Part3.Row = Part1.Row + 1;
            Part3.Column = Part1.Column + 1;

            Part4.Row = Part1.Row + 1;
            Part4.Column = Part1.Column + 2;
        }

        protected override void ChangeRotationToRotatedOnce()
        {
            RotationState = RotationState.RotatedOnce;

            ComposedShape = new[,]
                    {
                        { null,       Part1,      null,       null },
                        { Part2,      Part3,      null,       null },
                        { Part4,      null,       null,       null },
                        { null,       null,       null,       null }

                    };

            Part1.Row = Row - 1;
            Part1.Column = Column;

            Part2.Row = Part1.Row + 1;
            Part2.Column = Part1.Column - 1;

            Part3.Row = Part1.Row + 1;
            Part3.Column = Part1.Column;

            Part4.Row = Part1.Row + 2;
            Part4.Column = Part1.Column - 1;
        }


        protected override void ChangeRotationToRotatedTwice()
        {
            RotationState = RotationState.RotatedTwice;

            ComposedShape = new[,]
                    {
                        { Part1,      Part2,      null,       null },
                        { null,       Part3,      Part4,      null },
                        { null,       null,       null,       null },
                        { null,       null,       null,       null }
                    };

            Part1.Row = Row - 1;
            Part1.Column = Column - 2;

            Part2.Row = Part1.Row;
            Part2.Column = Part1.Column + 1;

            Part3.Row = Part1.Row + 1;
            Part3.Column = Part1.Column + 1;

            Part4.Row = Part1.Row + 1;
            Part4.Column = Part1.Column + 2;
        }

        protected override void ChangeRotationToRotatedThreeTimes()
        {
            RotationState = RotationState.RotatedThreeTimes;

            ComposedShape = new [,]
                    {
                        { null,       Part1,      null,      null },
                        { Part2,      Part3,      null,      null },
                        { Part4,      null,       null,      null },
                        { null,       null,       null,      null }

                    };

            Part1.Row = Row - 2;
            Part1.Column = Column + 1;

            Part2.Row = Part1.Row + 1;
            Part2.Column = Part1.Column - 1;

            Part3.Row = Part1.Row + 1;
            Part3.Column = Part1.Column;

            Part4.Row = Part1.Row + 2;
            Part4.Column = Part1.Column - 1;
        }

        #endregion Overriden Methods
    }
}