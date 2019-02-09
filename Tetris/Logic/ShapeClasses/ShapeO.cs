using Tetris.Logic.Enums;

namespace Tetris.Logic.ShapeClasses
{
    public sealed class ShapeO : Shape
    {

        #region Constructor
        
        public ShapeO(int row, int column) : base(row, column)
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

            Part1.Row = Row;
            Part1.Column = Column;

            Part2.Row = Part1.Row + 1;
            Part2.Column = Part1.Column;

            Part3.Row = Part1.Row;
            Part3.Column = Part1.Column + 1;

            Part4.Row = Part1.Row + 1;
            Part4.Column = Part1.Column + 1;
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
