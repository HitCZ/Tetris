using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris {
    class ShapeO : Shape {
        public ShapeO(int row, int column, Game game) : base(row, column, game) {
        }

        protected override void InitializeShape() {
            this.type = ShapeType.ShapeO;
            this.NumberOfRotationStates = 1;
            this.ChangeRotationToDefault();
        }

        protected override void ChangeRotationToDefault() {
            this.RotationState = RotationState.Default;

            this.ComposedShape = new Block[4, 4]
            {
                { this.Part1, this.Part2, null, null },
                { this.Part3, this.Part4, null, null },
                { null,       null,       null, null },
                { null,       null,       null, null }
            };

            this.Part1.Row = this.Row;
            this.Part1.Column = this.Column;

            this.Part2.Row = this.Part1.Row + 1;
            this.Part2.Column = this.Part1.Column;

            this.Part3.Row = this.Part1.Row;
            this.Part3.Column = this.Part1.Column + 1;

            this.Part4.Row = this.Part1.Row + 1;
            this.Part4.Column = this.Part1.Column + 1;
        }

        protected override void ChangeRotationToRotatedOnce() {
        }

        protected override void ChangeRotationToRotatedThreeTimes() {
        }

        protected override void ChangeRotationToRotatedTwice() {
        }
    }
}
