using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL
{
    public partial class Form1 : Form
    {
        // The universe array
        static int arrx = 5;
        static int arry = 5;

        bool[,] universe = new bool[arrx, arry];
        bool[,] scratchPad = new bool[arrx, arry];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }
        private int CountNeighbors(int x, int y)
        {
            int Neighbors = 0;
            int Left = x - 1;
            int Right = x + 1;
            int Up = y - 1;
            int Down = y + 1;

            //Checks for Neighbors in one general direction
            if (Up >= 0)
            {
                if (universe[x, Up] == true)
                {
                    Neighbors += 1;
                }
            }
            if (Left >= 0)
            {
                if (universe[Left, y] == true)
                {
                    Neighbors += 1;
                }
            }
            if (Down < arry)
            {
                if (universe[x, Down] == true)
                {
                    Neighbors += 1;
                }
            }
            if (Right < arrx)
            {
                if (universe[Right, y] == true)
                {
                    Neighbors += 1;
                }
            }

            //Checks in Diagonal Directions
            //Left side Diagonals
            if (Left >= 0 && Up >= 0)
            {
                if (universe[Left, Up] == true)
                {
                    Neighbors += 1;
                }
            }

            if (Left >= 0 && Down < arry)
            {
                if (universe[Left, Down] == true)
                {
                    Neighbors += 1;
                }
            }

            //Right side Diagonals 

            if (Right < arrx && Up >= 0)
            {
                if (universe[Right, Up] == true)
                {
                    Neighbors += 1;
                }
            }

            if (Right < arrx && Down < arry)
            {
                if (universe[Right, Down] == true)
                {
                    Neighbors += 1;
                }
            }

            return Neighbors;
        }
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for (int i = 0; i < arry; i++)
            {
                for (int k = 0; k < arrx; k++)
                {
                    scratchPad[k, i] = universe[k, i];
                }
            }

            
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighbors = CountNeighbors(x,y);
                    if ((neighbors < 2 || neighbors > 3) && universe[x, y] == true)
                    {
                        scratchPad[x, y] = false;
                    }
                    if ( (neighbors == 3) && universe[x, y] == false  )
                    {
                        scratchPad[x, y] = true;
                    }
                }
            }

            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    
                    int neighbors = CountNeighbors(x,y);
                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        Font font = new Font("Arial", cellHeight/2);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        
                        if((neighbors < 2 || neighbors > 3) && universe[x,y] == true)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                        }
                        else if((neighbors >= 2 || neighbors <= 3) && universe[x,y] == true)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                        }
                       
                        
                    }

                    if(neighbors > 0)
                    {
                        Font font = new Font("Arial", cellHeight / 2);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        
                        // green in alive and red is dead
                        if(neighbors == 3 && universe[x,y] == false)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                        }
                        else if( universe[x,y] == false)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                        }
                        
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
            timer.Enabled = false;
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
            timer.Enabled = false;
        }
    }
}
