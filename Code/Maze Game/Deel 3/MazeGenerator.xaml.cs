using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MazeGame
{
    /// <summary>
    /// Interaction logic for MazeGenerator.xaml
    /// </summary>
    public partial class MazeGenerator : Window
    {
        private struct Cell
        {
            public CellType Type;
            public int X;
            public int Y;

            public override string ToString() => Type.ToString();
        }

        private enum CellType
        {
            W,
            U,
            O,
            F
        }

        private Random rnd = new Random();
        private SaveFileDialog sfd = new SaveFileDialog() { Filter = "json files (*.json)|*.json", RestoreDirectory = true, FileName = "newMaze.json" };
        private Cell[,] maze;

        public MazeGenerator()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Init Maze
            var x = (2 * int.Parse(TbSizeX.Text)) + 1;
            var y = (2 * int.Parse(TbSizeY.Text)) + 1;
            maze = new Cell[x, y];
            List<Cell> Unopened = new List<Cell>();
            List<Cell> Frontiers = new List<Cell>();

            // Set all on Unopened
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    maze[i, j].X = i;
                    maze[i, j].Y = j;
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        maze[i, j].Type = CellType.U;
                        Unopened.Add(maze[i, j]);
                    }
                }
            }

            // Start randomly
            var current = Unopened[rnd.Next(Unopened.Count)];
            Unopened.Remove(current);
            maze[current.X, current.Y].Type = CellType.O;
            FrontierSurrounding(current, ref Frontiers, ref Unopened);

            while (Unopened.Count != 0 || Frontiers.Count != 0)
            {
                // Deal with a frontier
                current = DoFrontier(Frontiers[rnd.Next(Frontiers.Count)], ref Frontiers, ref Unopened);
                FrontierSurrounding(current, ref Frontiers, ref Unopened);
            }

            PrintMaze();
        }

        private Cell DoFrontier(Cell current, ref List<Cell> frontiers, ref List<Cell> unopened)
        {
            var x = maze.GetLength(0);
            var y = maze.GetLength(1);
            maze[current.X, current.Y].Type = CellType.O;
            frontiers.Remove(frontiers.Where(t => t.X == current.X && t.Y == current.Y).ToList()[0]);

            List<Cell> pos = new List<Cell>();

            // Right
            if (current.X + 2 < x) if (maze[current.X + 2, current.Y].Type == CellType.O) pos.Add(maze[current.X + 2, current.Y]);

            // Left
            if (current.X - 2 > 0) if (maze[current.X - 2, current.Y].Type == CellType.O) pos.Add(maze[current.X - 2, current.Y]);

            // Top
            if (current.Y + 2 < y) if (maze[current.X, current.Y + 2].Type == CellType.O) pos.Add(maze[current.X, current.Y + 2]);

            // Bottom
            if (current.Y - 2 > 0) if (maze[current.X, current.Y - 2].Type == CellType.O) pos.Add(maze[current.X, current.Y - 2]);

            var toCon = pos[rnd.Next(pos.Count)];

            var betw_x = (current.X > toCon.X) ? toCon.X + 1 : ((current.X == toCon.X) ? toCon.X : toCon.X - 1);
            var betw_y = (current.Y > toCon.Y) ? toCon.Y + 1 : ((current.Y == toCon.Y) ? toCon.Y : toCon.Y - 1);
            maze[betw_x, betw_y].Type = CellType.O;

            return current;
        }

        private void PrintMaze()
        {
            TbOutput.Text = "";
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    TbOutput.Text += ((maze[i, j].Type == CellType.W) ? "." : "█");
                }
                TbOutput.Text += "\n";
            }
        }

        private void FrontierSurrounding(Cell current, ref List<Cell> frontiers, ref List<Cell> unopened)
        {
            var x = maze.GetLength(0);
            var y = maze.GetLength(1);

            // Right
            if (current.X + 2 < x)
            {
                if (maze[current.X + 2, current.Y].Type == CellType.U)
                {
                    maze[current.X + 2, current.Y].Type = CellType.F;
                    frontiers.Add(maze[current.X + 2, current.Y]);
                    unopened.Remove(unopened.Where(t => t.X == current.X + 2 && t.Y == current.Y).ToList()[0]);
                }
            }

            // Left
            if (current.X - 2 > 0)
            {
                if (maze[current.X - 2, current.Y].Type == CellType.U)
                {
                    maze[current.X - 2, current.Y].Type = CellType.F;
                    frontiers.Add(maze[current.X - 2, current.Y]);
                    unopened.Remove(unopened.Where(t => t.X == current.X - 2 && t.Y == current.Y).ToList()[0]);
                }
            }

            // Top
            if (current.Y + 2 < y)
            {
                if (maze[current.X, current.Y + 2].Type == CellType.U)
                {
                    maze[current.X, current.Y + 2].Type = CellType.F;
                    frontiers.Add(maze[current.X, current.Y + 2]);
                    unopened.Remove(unopened.Where(t => t.X == current.X && t.Y == current.Y + 2).ToList()[0]);
                }
            }

            // Bottom
            if (current.Y - 2 > 0)
            {
                if (maze[current.X, current.Y - 2].Type == CellType.U)
                {
                    maze[current.X, current.Y - 2].Type = CellType.F;
                    frontiers.Add(maze[current.X, current.Y - 2]);
                    unopened.Remove(unopened.Where(t => t.X == current.X && t.Y == current.Y - 2).ToList()[0]);
                }
            }
        }

        private void ToJson()
        {
            var data = new MazeData
            {
                Size = new List<int>() { maze.GetLength(1) * 2, maze.GetLength(0) * 2, 2 },
                Spawn = new List<double>() { (-maze.GetLength(1)) + 3, (maze.GetLength(0)) - 2, .5 },
                Goal = new List<double>() { (maze.GetLength(1)) - 3, (maze.GetLength(0)) - 2, .5 },
                Title = "INSERT TITLE HERE",
                CodeSingle = GetCodeSingle()
            };

            var json = JsonConvert.SerializeObject(data);
            sfd.ShowDialog();

            File.WriteAllText(sfd.FileName, json);

            Close();
        }

        private string GetCodeSingle()
        {
            string returnable = "";
            for (int i = 0; i < maze.GetLength(0) * maze.GetLength(1) * 4; i++) returnable += "G";
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                string line = "";
                for (int j = 0; j < maze.GetLength(1); j++) line += (maze[i, j].Type == CellType.W) ? "WW" : "EE";
                returnable += line + line;
            }

            // add in spawn and finish
            var builder = new StringBuilder(returnable);
            builder[(maze.GetLength(1) * 2 * 2) + 2] = 'B';
            builder[(maze.GetLength(1) * 2 * 2) + 3] = 'B';
            builder[(maze.GetLength(1) * 2 * 3) - 3] = 'F';
            builder[(maze.GetLength(1) * 2 * 3) - 4] = 'F';
            builder[(maze.GetLength(1) * 2 * 3) + 2] = 'B';
            builder[(maze.GetLength(1) * 2 * 3) + 3] = 'B';
            builder[(maze.GetLength(1) * 2 * 4) - 3] = 'F';
            builder[(maze.GetLength(1) * 2 * 4) - 4] = 'F';
            return builder.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) => ToJson();
    }
}
