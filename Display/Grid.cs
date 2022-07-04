using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Display
{
    /// <summary>
    /// Statically sized console window with a programmable grid. Updates to 
    /// the grid can be specified and rendered separately when requested.
    /// </summary>
    /// <author>Benjamin Lewis (2020) & Dan Tran (2022)</author>
    /// <created>August 2020</created>
    /// <updated>June 2022</updated>
    public class Grid
    {
        private const int TopMargin = 1;
        private const int BottomMargin = 0;
        private const int LeftMargin = 1;
        private const int RightMargin = 0;
        private const int Border = 1;
        private const int MinRow = 8;
        private const int MaxRow = 32;
        private const int MinCol = 16;
        private const int MaxCol = 64;
        private const ConsoleColor DefaultConsoleColor = ConsoleColor.White;

        private int rows;
        private int cols;
        private int bufferHeight;
        private int bufferWidth;
        private string footnote;
        private char[][] buffer;
        private ConsoleColor[][] colorBuffer;
        private readonly Queue<Point> renderQueue;

        public bool IsComplete { get; set; }

        #if WINDOWS
                [DllImport("kernel32.dll", ExactSpelling = true)]
                private static extern IntPtr GetConsoleWindow();
        #else
                [DllImport("libc")]
                private static extern int system(string exec);
        #endif

        /// <summary>
        /// Instantiate a grid with a fixed number of rows and columns.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the grid size exceed pre-defined limits.</exception>
        public Grid(int rows, int cols)
        {
            if (rows < MinRow || rows > MaxRow)
            {
                throw new ArgumentOutOfRangeException($"The number of grid rows is not within the acceptable range " +
                                                      $"of values ({MinRow} to {MaxRow}).");
            }

            if (cols < MinCol || cols > MaxCol)
            {
                throw new ArgumentOutOfRangeException($"The number of grid columns is not within the acceptable range " +
                                                      $"of values ({MinCol} to {MaxCol}).");
            }

            this.rows = rows;
            this.cols = cols;
            this.renderQueue = new Queue<Point>();
            this.IsComplete = false;

            CalculateBufferSize();
            InitializeBuffer();
            DrawBorder();
        }

        /// <summary>
        /// Resizes the window to the appropriate size and clears the console.
        /// </summary>
        public void InitializeWindow()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.Clear();
        }

        /// <summary>
        /// Renders the current state of the grid (all updates applied after the last render will be rendered).
        /// </summary>
        public void Render()
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            while (renderQueue.Count > 0)
            {
                Point pixel = renderQueue.Dequeue();
                int row = pixel.Row, col = pixel.Column;
                Console.SetCursorPosition(col, row);
                char c = buffer[row][col];
                Console.ForegroundColor = colorBuffer[row][col];
                Console.Write(c);
            }
            Console.ForegroundColor = currentColor;
        }

        /// <summary>
        /// Fill a pixel on the grid with a character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <param name="color">The color to set to.</param>
        /// <param name="row">The row index of the pixel.</param>
        /// <param name="col">The column index of the pixel.</param>
        public void FillPixel(char c, ConsoleColor color, int row, int col)
        {
            if (buffer[row + CellRowOffset(row)][col + CellColOffset(col)] == c &&
                colorBuffer[row + CellRowOffset(row)][col + CellColOffset(col)] == color) return;
            buffer[row + CellRowOffset(row)][col + CellColOffset(col)] = c;
            colorBuffer[row + CellRowOffset(row)][col + CellColOffset(col)] = color;
            renderQueue.Enqueue(new Point(row + CellRowOffset(row), col + CellColOffset(col)));
        }

        /// <summary>
        /// Clear the grid.
        /// </summary>
        public void Clear()
        {
            InitializeBuffer();
            DrawBorder();
            Render();
        }

        /// ------------------------------------------------------------
        /// Private Methods. These CANNOT be called from your program.
        /// ------------------------------------------------------------

        /// <summary>
        /// Initializes the buffer array to be filled with whitespace.
        /// </summary>
        private void InitializeBuffer()
        {
            buffer = new char[bufferHeight][];
            for (int i = 0; i < bufferHeight; i++)
            {
                buffer[i] = new char[bufferWidth];
                for (int j = 0; j < bufferWidth; j++)
                {
                    buffer[i][j] = ' ';
                    renderQueue.Enqueue(new Point(i, j));
                }
            }
            colorBuffer = new ConsoleColor[bufferHeight][];
            for (int i = 0; i < bufferHeight; i++)
            {
                colorBuffer[i] = new ConsoleColor[bufferWidth];
                for (int j = 0; j < bufferWidth; j++)
                {
                    colorBuffer[i][j] = DefaultConsoleColor;
                }
            }
        }

        /// <summary>
        /// Draws border characters at the appropriate buffer locations.
        /// </summary>
        private void DrawBorder()
        {
            if (Border == 1)
            {
                buffer[TopMargin][LeftMargin] = '╔';
                buffer[TopMargin][LeftMargin + Border + cols] = '╗';
                buffer[TopMargin + Border + rows][LeftMargin] = '╚';
                buffer[TopMargin + Border + rows][LeftMargin + Border + cols] = '╝';
                for (int i = TopMargin + Border; i <= (TopMargin + Border * rows); i++)
                {
                    buffer[i][LeftMargin] = '║';
                    buffer[i][LeftMargin + Border + cols] = '║';
                }
                for (int j = LeftMargin + Border; j <= (LeftMargin + Border * cols); j++)
                {
                    buffer[TopMargin][j] = '═';
                    buffer[TopMargin + Border + rows][j] = '═';
                }

                colorBuffer[TopMargin][LeftMargin] = DefaultConsoleColor;
                colorBuffer[TopMargin][LeftMargin + Border + cols] = DefaultConsoleColor;
                colorBuffer[TopMargin + Border + rows][LeftMargin] = DefaultConsoleColor;
                colorBuffer[TopMargin + Border + rows][LeftMargin + Border + cols] = DefaultConsoleColor;
                for (int i = TopMargin + Border; i <= (TopMargin + Border * rows); i++)
                {
                    colorBuffer[i][LeftMargin] = DefaultConsoleColor;
                    colorBuffer[i][LeftMargin + Border + cols] = DefaultConsoleColor;
                }
                for (int j = LeftMargin + Border; j <= (LeftMargin + Border * cols); j++)
                {
                    colorBuffer[TopMargin][j] = DefaultConsoleColor;
                    colorBuffer[TopMargin + Border + rows][j] = DefaultConsoleColor;
                }
            }
        }

        /// <summary>
        /// Offsets a grid column index to a buffer column index with respect
        /// to the left margin, border and cell width.
        /// </summary>
        /// <param name="col">The grid column index</param>
        /// <returns>The offset buffer column index</returns>
        private int CellColOffset(int col)
        {
            return LeftMargin + Border;
        }

        /// <summary>
        /// Offsets a grid row index to a buffer row index with respect
        /// to the top margin, border and cell height.
        /// </summary>
        /// <param name="row">The grid row index</param>
        /// <returns>The offset buffer row index</returns>
        private int CellRowOffset(int row)
        {
            return TopMargin + Border;
        }

        /// <summary>
        /// Calculates the buffer size based on margins borders and cell counts/sizes.
        /// </summary>
        private void CalculateBufferSize()
        {
            bufferHeight = TopMargin + BottomMargin + 2 * Border + rows;
            bufferWidth = LeftMargin + RightMargin + 2 * Border + cols;
        }
    }
}
