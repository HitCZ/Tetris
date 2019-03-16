using Tetris.Logic;
using Tetris.Logic.Enums;

namespace Tetris.Models.Shapes
{
    public sealed class ShapeO : Shape
    {

        #region Constructor
        
        public ShapeO(Position position) : base(position)
        {
            Initialize();
        }

        #endregion Constructor

        #region Overriden Methods
        
        protected override void Initialize()
        {
            Type = ShapeType.ShapeO;
            NumberOfRotationStates = 1;
            ChangeRotationToDefault();
        }

        protected override void ChangeRotationToDefault()
        {
            RotationState = RotationState.Default;

            ComposedShape = new[,]
            {
                { Part1,      Part2,      null, null },
                { Part3,      Part4,      null, null },
                { null,       null,       null, null },
                { null,       null,       null, null }
            };

            Part1.Position = Position;
            Part2.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column);
            Part3.Position = new Position(Part1.Position.Row, Part1.Position.Column + 1);
            Part4.Position = new Position(Part1.Position.Row + 1, Part1.Position.Column + 1);
        }

        protected override void ChangeRotationToRotatedOnce()
        {
        }

        protected override void ChangeRotationToRotatedThreeTimes()
        {
        }

        protected override void ChangeRotationToRotatedTwice()
        {
        }

        #endregion Overriden Methods
    }
}
