using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Display;


namespace ConsoleGame
{
    public abstract class Entity
    {
        private static readonly int _counter = 0;
        private ConsoleColor[][] colorMap;
        private Point currentLocation;
        private Point previousLocation;
        private bool isExpiring;
        protected int internalCounter;

        /// <summary>
        /// Checks if the entity is to be deleted.
        /// </summary>
        public virtual bool IsExpiring { get => isExpiring; set => isExpiring = value; }



        public int Id { get; }
        /// <summary>
        /// The characters building up the entity
        /// </summary>
        public char[][] Shape { get; protected set; }

        /// <summary>
        /// The color map corresponding to the entity's shape
        /// </summary>
        public ConsoleColor[][] ColorMap
        {
            get
            {
                bool matchedDimensions = true;
                for (int row = 0; row < Shape.Length; row++)
                {
                    matchedDimensions &= Shape[row].Length == colorMap?[row].Length;
                    if (!matchedDimensions) break;
                }
                if (!matchedDimensions) SetColor();
                return colorMap;
            }
            set => colorMap = value;
        }

        protected Game game;
        /// <summary>
        /// The velocity of the entity.
        /// </summary>
        public Point Velocity { get; protected set; }

        /// <summary>
        /// The location of the top-left pixel of the entity.
        /// </summary>
        public Point CurrentLocation
        {
            get => currentLocation;
            set
            {
                previousLocation = currentLocation;
                int maxRow = game.MaxRow - Shape.Length + 1;
                int maxCol = game.MaxCol - Shape.ToList().Select(x => x.Length).Max() + 1;
                int newRow = value.Row > 0 ? (value.Row < maxRow ? value.Row : maxRow) : 0;
                int newCol = value.Column > 0 ? (value.Column < maxCol ? value.Column : maxCol) : 0;
                Point newPoint = new Point(newRow, newCol);
                currentLocation = newPoint;
            }
        }

        /// <summary>
        /// Auto-increments _counter for entity ID assignment
        /// </summary>
        static Entity()
        {
            _counter++;
        }

        /// <summary>
        /// Defines an entity with an ID
        /// </summary>
        /// <param name="game">The game where this entity belongs</param>
        protected Entity(Game game)
        {
            this.Id = _counter;
            this.game = game;
            this.currentLocation = new Point(0, 0);
            this.previousLocation = new Point(0, 0);
            this.internalCounter = 0;
        }

        /// <summary>
        /// Draws the entity on the game grid
        /// </summary>
        public void Draw()
        {
            ErasePreviousImage();
            for (int row = 0; row < Shape.Length; row++)
            {
                char[] rowChars = Shape[row];
                for (int col = 0; col < rowChars.Length; col++)
                {
                    char c = rowChars[col];
                    ConsoleColor color = ColorMap[row][col];
                    game.GameGrid.FillPixel(c, color, currentLocation.Row + row, currentLocation.Column + col);
                }
            }
        }

        /// <summary>
        /// Erase the image of the entity at the previous location.
        /// </summary>
        public void ErasePreviousImage()
        {
            for (int row = 0; row < Shape.Length; row++)
            {
                char[] rowChars = Shape[row];
                for (int col = 0; col < rowChars.Length; col++)
                {
                    game.GameGrid.FillPixel(' ', ConsoleColor.Black, previousLocation.Row + row, previousLocation.Column + col);
                }
            }
        }

        /// <summary>
        /// Set the color map of the entity to a single color, ensuring the dimensions between the color map and the shape are matched
        /// </summary>
        /// <param name="color">The color to set. Default to white</param>
        public void SetColor(ConsoleColor color = ConsoleColor.White)
        {
            colorMap = new ConsoleColor[Shape.Length][];
            for (int row = 0; row < Shape.Length; row++)
            {
                colorMap[row] = new ConsoleColor[Shape[row].Length];
                for (int col = 0; col < Shape[row].Length; col++)
                {
                    colorMap[row][col] = color;
                }
            }
        }

        /// <summary>
        /// Update the location of the entity based on its velocity
        /// </summary>
        public virtual void NextFrame()
        {
            Point newLocation = CurrentLocation.Shift(Velocity);
            //int maxRow = game.MaxRow - Shape.Length + 1;
            //int maxCol = game.MaxCol - Shape.ToList().Select(x => x.Length).Max() + 1;
            //bool rowInBound = newLocation.Row >= 0 && newLocation.Row <= maxRow;
            //bool colInBound = newLocation.Column >= 0 && newLocation.Column <= maxCol;
            CurrentLocation = newLocation;
            internalCounter++;
        }

        /// <summary>
        /// Checks if the entity is going out of bound.
        /// </summary>
        /// <returns>true if the entity is going out of bound, false otherwise</returns>
        public virtual bool IsOutOfBound()
        {
            Point newLocation = CurrentLocation.Shift(Velocity);
            int maxRow = game.MaxRow - Shape.Length + 1;
            int maxCol = game.MaxCol - Shape.ToList().Select(x => x.Length).Max() + 1;
            bool rowInBound = newLocation.Row >= 0 && newLocation.Row <= maxRow;
            bool colInBound = newLocation.Column >= 0 && newLocation.Column <= maxCol;
            return !(rowInBound && colInBound);
        }

        /// <summary>
        /// Checks if the entity collides with another entity.
        /// </summary>
        /// <param name="other">The other entity.</param>
        /// <returns>true if the entities collide, false otherwise.</returns>
        public bool IsColliding(Entity other)
        {
            int colCheckStart = other.CurrentLocation.Column;
            int colCheckEnd = other.CurrentLocation.Column + other.Shape.Select(row => row.Length).Max();
            int rowCheckStart = other.CurrentLocation.Row;
            int rowCheckEnd = other.CurrentLocation.Row + other.Shape.Length;
            int columnSpan = Shape.Select(row => row.Length).Max();
            int rowSpan = Shape.Length;
            bool columnsCollided = (CurrentLocation.Column >= colCheckStart && CurrentLocation.Column < colCheckEnd)
                || (CurrentLocation.Column + columnSpan >= colCheckStart && CurrentLocation.Column + columnSpan < colCheckEnd);
            bool rowsCollided = CurrentLocation.Row >= rowCheckStart && CurrentLocation.Row < rowCheckEnd
                || (CurrentLocation.Row + rowSpan >= rowCheckStart && CurrentLocation.Row + rowSpan < rowCheckEnd);
            return columnsCollided && rowsCollided;
        }
    }
}
