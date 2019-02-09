using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.ShapeClasses {
    class ShapeZ : Shape {
        public ShapeZ(int row, int column, Game game) : base(row, column, game) {
        }

        protected override void InitializeShape() {
            this.type = ShapeType.ShapeZ;
            this.NumberOfRotationStates = 2;
            this.ChangeRotationToDefault();
        }
        
        protected override void ChangeRotationToDefault() {
            this.RotationState = RotationState.Default;

            this.ComposedShape = new Block[4, 4]
                    {
                        { this.Part1, this.Part2, null,       null },
                        { null,       this.Part3, this.Part4, null },
                        { null,       null,       null,       null },
                        { null,       null,       null,       null }
                    };

            this.Part1.Row = this.Row;
            this.Part1.Column = this.Column;

            this.Part2.Row = this.Part1.Row;
            this.Part2.Column = this.Part1.Column + 1;

            this.Part3.Row = this.Part1.Row + 1;
            this.Part3.Column = this.Part1.Column + 1;

            this.Part4.Row = this.Part1.Row + 1;
            this.Part4.Column = this.Part1.Column + 2;
        }

        protected override void ChangeRotationToRotatedOnce() {
            this.RotationState = RotationState.RotatedOnce;

            this.ComposedShape = new Block[4, 4]
                    {
                        { null,       this.Part1, null, null },
                        { this.Part2, this.Part3, null, null },
                        { this.Part4, null,       null, null },
                        { null,       null,       null, null }

                    };

            this.Part1.Row = this.Row - 1;
            this.Part1.Column = this.Column;

            this.Part2.Row = this.Part1.Row + 1;
            this.Part2.Column = this.Part1.Column - 1;

            this.Part3.Row = this.Part1.Row + 1;
            this.Part3.Column = this.Part1.Column;

            this.Part4.Row = this.Part1.Row + 2;
            this.Part4.Column = this.Part1.Column - 1;
        }


        protected override void ChangeRotationToRotatedTwice() {
            this.RotationState = RotationState.RotatedTwice;

            this.ComposedShape = new Block[4, 4]
                    {
                        { this.Part1, this.Part2, null,       null },
                        { null,       this.Part3, this.Part4, null },
                        { null,       null,       null,       null },
                        { null,       null,       null,       null }
                    };

            this.Part1.Row = this.Row - 1;
            this.Part1.Column = this.Column - 2;

            this.Part2.Row = this.Part1.Row;
            this.Part2.Column = this.Part1.Column + 1;

            this.Part3.Row = this.Part1.Row + 1;
            this.Part3.Column = this.Part1.Column + 1;

            this.Part4.Row = this.Part1.Row + 1;
            this.Part4.Column = this.Part1.Column + 2;
        }

        protected override void ChangeRotationToRotatedThreeTimes() {
            this.RotationState = RotationState.RotatedThreeTimes;

            this.ComposedShape = new Block[4, 4]
                    {
                        { null,       this.Part1, null, null },
                        { this.Part2, this.Part3, null, null },
                        { this.Part4, null,       null, null },
                        { null,       null,       null, null }

                    };

            this.Part1.Row = this.Row - 2;
            this.Part1.Column = this.Column + 1;

            this.Part2.Row = this.Part1.Row + 1;
            this.Part2.Column = this.Part1.Column - 1;

            this.Part3.Row = this.Part1.Row + 1;
            this.Part3.Column = this.Part1.Column;

            this.Part4.Row = this.Part1.Row + 2;
            this.Part4.Column = this.Part1.Column - 1;
        }
    }
}
