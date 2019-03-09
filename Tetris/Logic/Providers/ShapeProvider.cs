using System;
using Tetris.Logic.Enums;
using Tetris.Models.Shapes;

namespace Tetris.Logic.Providers
{
    public static class ShapeProvider
    {

        public static Shape GetRandomShape(int row, int column)
        {
            var random = new Random();
            var values = Enum.GetValues(typeof(ShapeType));
            var randomIndex = random.Next(values.Length);
            var randomShapeType = (ShapeType)values.GetValue(randomIndex);

            return ShapeFactory.GetShape(randomShapeType, row, column);
        }

        public static Shape GetConcreteShape(ShapeType type, int row, int column)
            => ShapeFactory.GetShape(type, row, column);
    }
}
