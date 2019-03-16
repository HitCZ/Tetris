using System;
using Tetris.Logic.Enums;
using Tetris.Models.Shapes;

namespace Tetris.Logic.Providers
{
    public static class ShapeProvider
    {

        public static Shape GetRandomShape(Position position)
        {
            var random = new Random();
            var values = Enum.GetValues(typeof(ShapeType));
            var randomIndex = random.Next(values.Length);
            var randomShapeType = (ShapeType)values.GetValue(randomIndex);

            return ShapeFactory.GetShape(randomShapeType, position);
        }

        public static Shape GetConcreteShape(ShapeType type, Position position) => ShapeFactory.GetShape(type, position);
    }
}
