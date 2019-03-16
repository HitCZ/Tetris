using System;
using Tetris.Logic.Enums;
using Tetris.Models.Shapes;

namespace Tetris.Logic
{
    public static class ShapeFactory
    {
        public static Shape GetShape(ShapeType type, Position position)
        {
            switch (type)
            {
                case ShapeType.ShapeL:
                    return new ShapeL(position);
                case ShapeType.ShapeLInverted:
                    return new ShapeLInverted(position);
                case ShapeType.ShapeI:
                    return new ShapeI(position);
                case ShapeType.ShapeO:
                    return new ShapeO(position);
                case ShapeType.ShapeZ:
                    return new ShapeZ(position);
                case ShapeType.ShapeS:
                    return new ShapeS(position);
                case ShapeType.ShapeT:
                    return new ShapeT(position);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
