using Tetris.Logic;
using Tetris.Logic.Enums;

namespace Tetris.Models.Shapes
{
    public sealed class ShapeI : Shape
    {
        #region Constructor

        public ShapeI(Position position) : base(position)
        {
            Initialize();
        }

        #endregion Constructor 

        #region Overriden Methods

        protected override void Initialize()
        {
            Type = ShapeType.ShapeI;
            NumberOfRotationStates = 2;
            ChangeRotationToDefault();
        }

        protected override void ChangeRotationToDefault()
        {
            RotationState = RotationState.Default;

            ComposedShape = new[,]
            {
                { Part1, null, null, null },
                { Part2, null, null, null },
                { Part3, null, null, null },
                { Part4, null, null, null }
            };

            Part1.Position = Position;
            Part2.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column);
            Part3.Position = new Position(Part1.Position.Row + 2, Part1.Position.Column);
            Part4.Position = new Position(Part1.Position.Row + 3, Part1.Position.Column);
        }

        protected override void ChangeRotationToRotatedOnce()
        {
            RotationState = RotationState.RotatedOnce;

            ComposedShape = new[,]
            {
                { Part1,      Part2,      Part3,      Part4},
                { null,       null,       null,       null },
                { null,       null,       null,       null },
                { null,       null,       null,       null }
            };

            Part1.Position = new Position(Position.Row + 3, Position.Column - 1);
            Part2.Position = new Position(Part1.Position.Row, Part1.Position.Column + 1);
            Part3.Position = new Position(Part1.Position.Row, Part1.Position.Column + 2);
            Part4.Position = new Position(Part1.Position.Row, Part1.Position.Column + 3);
        }

        protected override void ChangeRotationToRotatedTwice() => ChangeRotationToDefault();

        protected override void ChangeRotationToRotatedThreeTimes() => ChangeRotationToRotatedOnce();

        #endregion Overriden Methods
    }
}
