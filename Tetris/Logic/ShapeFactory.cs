using System;
using Tetris.Logic.Enums;
using Tetris.Models.Shapes;

namespace Tetris.Logic
{
    public static class ShapeFactory
    {
        public static Shape GetShape(ShapeType type, int row, int column)
        {
            switch (type)
            {
                case ShapeType.ShapeL:
                    return new ShapeL(row, column);
                case ShapeType.ShapeLInverted:
                    return new ShapeLInverted(row, column);
                case ShapeType.ShapeI:
                    return new ShapeI(row, column);
                case ShapeType.ShapeO:
                    return new ShapeO(row, column);
                case ShapeType.ShapeZ:
                    return new ShapeZ(row, column);
                case ShapeType.ShapeS:
                    return new ShapeS(row, column);
                case ShapeType.ShapeT:
                    return new ShapeT(row, column);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
