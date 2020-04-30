using System;
using System.IO;
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
using System.Runtime.InteropServices;

namespace GOL
{
    public partial class Form1 : Form
    {
        // The universe array
        static int arrx = 30;
        static int arry = 30;

        bool[,] universe = new bool[arrx, arry];
        bool[,] scratchPad = new bool[arrx, arry];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Variable to hold seed
        int seed = 0;

        // Alive Cell count
        int alive = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // Set to false to prevent timer from starting on startup

        }
        //counts cells that are alive

        private void CountAlive()
        {
            alive = 0;
            
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                        alive += 1;
                }
            }
            toolStripStatusAlive.Text = "Alive = " + alive.ToString();
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

                    int neighbors = CountNeighbors(x, y);
                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        Font font = new Font("Arial", cellHeight / 2);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        if ((neighbors < 2 || neighbors > 3) && universe[x, y] == true)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                        }
                        else if ((neighbors >= 2 || neighbors <= 3) && universe[x, y] == true)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                        }


                    }

                    if (neighbors > 0)
                    {
                        Font font = new Font("Arial", cellHeight / 2);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // green in alive and red is dead
                        if (neighbors == 3 && universe[x, y] == false)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                        }
                        else if (universe[x, y] == false)
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

            //calling CountAlive After drawing each time
            CountAlive();
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
        //Closes program
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // stars timer and next generation as play button
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        // Stops timer and also stops auto next generation. In other words pauses.
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        //Goes to next generation
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        // new button clears universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            //Goes through every cell in generation
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

        // Other new button
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

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsBox options = new OptionsBox();
            options.Timer = timer.Interval;
            options.width = arrx;
            options.height = arry;
            if(DialogResult.OK == options.ShowDialog())
            {
                timer.Interval = (int)options.Timer;

                arrx = (int)options.width;
                arry = (int)options.height;
                universe = new bool[arrx, arry];
                scratchPad = new bool[arrx, arry];
                graphicsPanel1.Invalidate();
            }
            
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Seed smenu = new Seed();

            smenu.seed = seed;
            if(DialogResult.OK == smenu.ShowDialog())
            {  
                
                seed = (int)smenu.seed;
                Random rng = new Random((int)smenu.seed);
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                       if (rng.Next(2) == 1)
                        universe[x, y] = true;
                       else    
                        universe[x, y] = false;
                          
                    }
                }
                
            }
               
            graphicsPanel1.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rng = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rng.Next(2) == 1)
                        universe[x, y] = true;
                    else
                        universe[x, y] = false;

                }
            }
            graphicsPanel1.Invalidate();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                
                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                   
                    // Create a string to represent the current row.
                    String currentRow = string.Empty; 
                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if( universe[x,y] == true)
                        {
                            currentRow += 'O';
                        }
                        else { currentRow += '.'; }

                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                
                    writer.WriteLine(currentRow);
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    if(row.First() != '!')
                    {
                        maxHeight += 1;
                        maxWidth = row.Length;
                    }
                  
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];
                arrx = maxWidth;
                arry = maxHeight;
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                int rowNum = 0;
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.First() != '!')
                    {
                        
                         for (int xPos = 0; xPos < row.Length; xPos++)
                            {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, rowNum] = true;
                            }
                            else
                            {
                                universe[xPos, rowNum] = false;
                            }
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                            }
                        rowNum += 1;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                   
                }

                // Close the file.
                reader.Close();
            }
            graphicsPanel1.Invalidate();

        }
    }
}
