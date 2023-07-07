using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region values
        readonly int size = 32;
        public int rows, columns, bombs, bombsRemaining;
        public GridCell[,] gridCells;

        enum TileValues
        {
            TileEmpty = 0,
            Tile1 = 1,
            Tile2 = 2,
            Tile3 = 3,
            Tile4 = 4,
            Tile5 = 5,
            Tile6 = 6,
            Tile7 = 7,
            Tile8 = 8,
            TileMine = 20,
        }
        #endregion

        public MainWindow() {
            InitializeComponent();
            GenerateGrid();
        }

        #region Generation
        private void GenerateGrid(object sender = null, RoutedEventArgs e = null) {


            if (!(int.TryParse(RowsCount.Text, out rows) && int.TryParse(ColumnCount.Text, out columns) && int.TryParse(BombsCount.Text, out bombs))) {
                MessageBox.Show("input valid numbers only");
                return;
            }

            GameGrid.Rows = rows;
            GameGrid.Columns = columns;
            GameGrid.Height = rows * size;
            GameGrid.Width = columns * size;

            GenerateGridValues();

            DrawGrid();


        }

        private void GenerateGridValues() {
            FlagsRemaining.Text = bombs.ToString();
            bombsRemaining = bombs;
            GenerateGameGrid();
            GenerateBombs();
            GenerateNumbers();
        }

        private void GenerateGameGrid() {
            gridCells = new GridCell[rows, columns];

            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {
                    gridCells[r, c] = new GridCell {
                        explored = false
                    };
                }
            }
        }

        private void GenerateBombs() {

            Random random = new Random();
            for (int i = 0; i < bombs; i++) {
                int x = random.Next(rows);
                int y = random.Next(columns);

                if (gridCells[x, y].value == 20) {
                    i--;
                    continue;
                }
                gridCells[x, y].value = 20;

            }
        }

        private void GenerateNumbers() {
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {

                    if (gridCells[r, c].value == 20) {
                        continue;
                    }
                    int bombCount = 0;

                    try {
                        bombCount += gridCells[r - 1, c - 1].value == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += gridCells[r - 1, c].value == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += gridCells[r - 1, c + 1].value == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += gridCells[r, c - 1].value == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += gridCells[r, c + 1].value == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += gridCells[r + 1, c - 1].value == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += gridCells[r + 1, c].value == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += gridCells[r + 1, c + 1].value == 20 ? 1 : 0;
                    } catch (Exception) { }

                    gridCells[r, c].value = bombCount;

                }
            }
        }
        #endregion
        private void DrawGrid() {
            GameGrid.Children.Clear();
            string adress = "";
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {

                    if (gridCells[r, c].flagged == true) {
                        adress = "Assets/TileFlag.png";
                    } else if (gridCells[r, c].explored == false) {
                        adress = "Assets/TileUnknown.png";
                    } else if (gridCells[r, c].explored == true) {
                        adress = $"Assets/{(TileValues)gridCells[r, c].value}.png";
                    }
                    BitmapImage image = new BitmapImage(new Uri(adress,UriKind.Relative));
                    GameGrid.Children.Add(new Image { Source = image});


                }
            }
        }


        #region Clicking
        private void PlaceFlag(object sender, MouseButtonEventArgs e) {
            Point Clickpos = e.GetPosition(GameGrid);
            int xPos = (int)Math.Floor(Clickpos.X / size);
            int yPos = (int)Math.Floor(Clickpos.Y / size);

            if (gridCells[yPos, xPos].explored == true) {
                return;
            }


            if (gridCells[yPos, xPos].flagged == true) {
                bombsRemaining++;
                FlagsRemaining.Text = bombsRemaining.ToString();
                gridCells[yPos, xPos].flagged = false;
            } else if (gridCells[yPos, xPos].flagged == false) {
                bombsRemaining--;
                FlagsRemaining.Text = bombsRemaining.ToString();

                gridCells[yPos, xPos].flagged = true;
            }
            DrawGrid();
            CheckIfWon();
        }

        private void ClickGameGrid(object sender, MouseEventArgs e) {

            Point Clickpos = e.GetPosition(GameGrid);
            int[] clickedTile = new int[2] { (int)Math.Floor(Clickpos.Y / size), (int)Math.Floor(Clickpos.X / size) };

            int value = gridCells[clickedTile[0], clickedTile[1]].value;
            bool explored = gridCells[clickedTile[0], clickedTile[1]].explored;

            gridCells[clickedTile[0], clickedTile[1]].explored = true;

            switch (value) {

                case 0:
                    FloodFill(clickedTile);
                    return;

                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    RevealNumbers(clickedTile, value, explored);
                    DrawGrid();
                    return;

                case 20:
                    YouLose();
                    return;
            }
        }
        #endregion

        private void FloodFill(int[] point) {
            List<int[]> newPoints = new List<int[]>();
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    try {
                        if (gridCells[point[0] + x, point[1] + y].value == 0 && gridCells[point[0] + x, point[1] + y].explored == false) {
                            newPoints.Add(new int[2] { point[0] + x, point[1] + y });
                        }
                        if (gridCells[point[0] + x, point[1] + y].explored == false) {
                            gridCells[point[0] + x, point[1] + y].explored = true;
                        }

                    } catch (Exception) {
                    }
                }
            }
            foreach (int[] newPoint in newPoints) {
                FloodFill(newPoint);
            }
            DrawGrid();
            CheckIfWon();
        }

        private void RevealNumbers(int[] clickedTile, int value, bool explored) {
            if (explored == true) {
                FlagRevealMethod(clickedTile, value);
            } else if (explored == false) {
                gridCells[clickedTile[0], clickedTile[1]].explored = true;
            }
            DrawGrid();
        }

        private void FlagRevealMethod(int[] clickedTile, int value) {
            int flagCount = 0;
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    try {
                        if (gridCells[clickedTile[0] + x, clickedTile[1] + y].flagged == true) {
                            flagCount++;
                        }
                    } catch (Exception) { }
                }
            }
            if (flagCount == value) {
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        try {
                            
                            if (gridCells[clickedTile[0] + x, clickedTile[1] + y].flagged == false) {
                                if (gridCells[clickedTile[0] + x, clickedTile[1] + y].value == 20) {
                                    YouLose();
                                    return;
                                }
                                gridCells[clickedTile[0] + x, clickedTile[1] + y].explored = true;
                            }
                        } catch (Exception) { }
                    }
                }
                DrawGrid();
                CheckIfWon();
            }
        }

        #region checks
        private void CheckIfWon() {
            if (gridCells.Cast<GridCell>().Count(b => b.flagged) - bombs == 0) {
                MessageBox.Show("YOU WIN!!!");
            }
        }
        private void YouLose() {
            
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {
                    gridCells[r, c].explored = true;
                }
            }

            DrawGrid();
            MessageBox.Show("You Lose!");
        }
        #endregion
    }
}
