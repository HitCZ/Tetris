using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris {
    /// <summary>
    /// Vycet predstavujici jednotlive tvary objektu
    /// </summary>
    public enum ShapeType {
        /// <summary>
        /// Tvar L
        /// </summary>
        ShapeL,

        ShapeLInverted,

        /// <summary>
        /// Tvar I (cara)
        /// </summary>
        ShapeI,

        /// <summary>
        /// Tvar O (kostka)
        /// </summary>
        ShapeO,

        /// <summary>
        /// Tvar Z
        /// </summary>
        ShapeZ,

        ShapeS,

        /// <summary>
        /// Tvar T
        /// </summary>
        ShapeT
    }
}
