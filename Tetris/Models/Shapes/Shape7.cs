using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.ShapeClasses {
    class Shape7 : Shape {
        public Shape7(int x, int y, Game game) : base(x, y, game) {
        }
    
        protected override void InitializeShape() {
            this.NumberOfStates = 4;
            this.ChangeRotationToDefault();
        }

        protected override void ChangeRotationToDefault() {
            this.RotationState = RotationState.Default;

            this.ComposedShape = new Block[4, 4]
                {
                    { null, this.Part1, this.Part2, null },
                    { null, this.Part3, null,       null },
                    { null, this.Part4, null,       null },
                    { null, null,       null,       null },
                };

            this.Part1.X = this.X;
            this.Part1.Y = this.Y;

            this.Part2.X = this.Part1.X + 1;
            this.Part2.Y = this.Part1.Y;

            this.Part3.X = this.Part1.X;
            this.Part3.Y = this.Part1.Y + 1;

            this.Part4.X = this.Part1.X;
            this.Part4.Y = this.Part1.Y + 2;
        }

        protected override void ChangeRotationToRotatedOnce() {
            this.RotationState = RotationState.RotatedOnce;

            this.ComposedShape = new Block[4, 4]
                {
                    { this.Part1, this.Part2, this.Part3, null },
                    { null,       null,       this.Part4, null },
                    { null,       null,       null,       null },
                    { null,       null,       null,       null },
                };

            this.Part1.X = this.X;
            this.Part1.Y = this.Y;

            this.Part2.X = this.Part1.X + 1;
            this.Part2.Y = this.Part1.Y;

            this.Part3.X = this.Part1.X + 2;
            this.Part3.Y = this.Part1.Y;

            this.Part4.X = this.Part1.X + 2;
            this.Part4.Y = this.Part1.Y + 1;
        }


        protected override void ChangeRotationToRotatedTwice() {
            this.RotationState = RotationState.RotatedTwice;

            this.ComposedShape = new Block[4, 4]
                {
                    { null,       this.Part1, null, null },
                    { null,       this.Part2, null, null },
                    { this.Part3, this.Part4, null, null },
                    { null,       null,       null, null },
                };

            this.Part1.X = this.X;
            this.Part1.Y = this.Y;

            this.Part2.X = this.Part1.X;
            this.Part2.Y = this.Part1.Y + 1;

            this.Part3.X = this.Part1.X - 1;
            this.Part3.Y = this.Part1.Y + 2;

            this.Part4.X = this.Part1.X;
            this.Part4.Y = this.Part1.Y + 2;
        }

        protected override void ChangeRotationToRotatedThreeTimes() {
            this.RotationState = RotationState.RotatedThreeTimes;

            this.ComposedShape = new Block[4, 4]
                {
                    { this.Part1, null,       null,       null },
                    { this.Part2, this.Part3, this.Part4, null },
                    { null,       null,       null,       null },
                    { null,       null,       null,       null },
                };

            this.Part1.X = this.X;
            this.Part1.Y = this.Y;

            this.Part2.X = this.Part1.X;
            this.Part2.Y = this.Part1.Y + 1;

            this.Part3.X = this.Part1.X + 1;
            this.Part3.Y = this.Part1.Y + 1;

            this.Part4.X = this.Part1.X + 2;
            this.Part4.Y = this.Part1.Y + 1;
        }
    }
}
