using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Display
{
    /// <summary>
    /// A point on the (Row, Column) coordinate system.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Read-only. The row index of the point, with 0 being the top most row. 
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// Read-only. The column index of the point, with 0 being the left most column. 
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Defines a point with a row and a column index.
        /// </summary>
        /// <param name="row">The row index of the point</param>
        /// <param name="column">The column index of the point</param>
        public Point(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Creates a point based on the current point, shifted by certain values.
        /// </summary>
        /// <param name="row">The row shift.</param>
        /// <param name="column">The column shift.</param>
        /// <param name="inPlace">Whether the current point is shifted.</param>
        /// <returns>The shifted point.</returns>
        public Point Shift(int row, int column, bool inPlace = false)
        {
            if (!inPlace) return new Point(Row + row, Column + column);
            Row += row;
            Column += column;
            return this;
        }

        /// <summary>
        /// Creates a point based on the current point, shifted by certain displacement vector.
        /// </summary>
        /// <param name="displacement">The displacement vector to shift by</param>
        /// <param name="inPlace">Whether the current point is shifted.</param>
        /// <returns>The shifted point.</returns>
        public Point Shift(Point displacement, bool inPlace = false)
        {
            return Shift(displacement.Row, displacement.Column, inPlace);
        }
    }
}
