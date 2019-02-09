using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Color = Tetris.Logic.Enums.Color;

namespace Tetris.Logic.Providers
{
    public static class ColorProvider
    {
        #region Fields

        private static Dictionary<Color, string> cache;

        #endregion Fields

        #region Constructor

        static ColorProvider()
        {
            Initialize();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Tries to get HEX for given color, if it cannot be found, returns empty string.
        /// </summary>
        public static string GetColorHex(Color color)
        {
            var isFound = cache.TryGetValue(color, out var value);

            return !isFound ? string.Empty : value;
        }

        /// <summary>
        /// Returns random color name.
        /// </summary>
        public static Color GetRandomColor()
        {
            var random = new Random();
            var randomIndex = random.Next(cache.Count);
            var randomColor = cache.ElementAt(randomIndex).Key;

            return randomColor;
        }

        /// <summary>
        /// Converts the given HEX to color and returns it.
        /// </summary>
        public static Brush GetBrushFromHex(string colorHex)
        {
            var converter = new BrushConverter();

            return (Brush)converter.ConvertFromString(colorHex);
        }

        public static Brush GetRandomBrush()
        {
            cache.TryGetValue(GetRandomColor(), out var value);

            return GetBrushFromHex(value);
        }

        #endregion Public Methods

        #region Private Methods

        private static void Initialize()
        {
            cache = new Dictionary<Color, string>
            {
                {Color.Blue, "#00d2ff"},
                {Color.DarkBlue, "#1c37ff"},
                {Color.Green, "#08ff4e"},
                {Color.Pink, "#ee2bff"},
                {Color.Yellow, "#f9e400"},
                {Color.Red, "#ff0000"},
                {Color.Orange, "#ff9600"},
                {Color.White, "#ffffff"},
                {Color.Purple, "#9740c9"}
            };
        }

        #endregion Private Methods
    }
}
