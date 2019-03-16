using Tetris.Logic;
using Tetris.Logic.Enums;

namespace Tetris.Models.Shapes
{
    public sealed class ShapeZ : Shape
    {

        #region Constructor

        public ShapeZ(Position position) : base(position)
        {
            Initialize();
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

            Part1.Position = Position;
            Part2.Position = new Position(Part1.Position.Row, Part1.Position.Column + 1);
            Part3.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column + 1);
            Part4.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column + 2);
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

            Part1.Position = new Position(Position.Row - 1, Position.Column);
            Part2.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column - 1);
            Part3.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column);
            Part4.Position = new Position(Part1.Position.Row + 2, Part1.Position.Column - 1);
        }


        protected override void ChangeRotationToRotatedTwice() => ChangeRotationToDefault();

        protected override void ChangeRotationToRotatedThreeTimes() => ChangeRotationToRotatedOnce();

        #endregion Overriden Methods
    }
}
