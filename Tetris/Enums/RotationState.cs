using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris {
    /// <summary>
    /// Predstavuji informaci o tom, kolikrat byl dany objekt otocen
    /// </summary>
    public enum RotationState {
        /// <summary>
        /// Vychozi stav rotace
        /// </summary>
        Default,

        /// <summary>
        /// Objekt byl otocen jednou
        /// </summary>
        RotatedOnce,

        /// <summary>
        /// Objekt byl otocen dvakrat
        /// </summary>
        RotatedTwice,

        /// <summary>
        /// Objekt byl otocen trikrat
        /// </summary>
        RotatedThreeTimes
    }
}
