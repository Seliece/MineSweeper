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
        int size = 32;
        public static int rows, columns, bombs, bombsRemaining;
        public static int[,] GameGridValues;
        public static bool?[,] GameGridExplored;

        enum TileValues
        {
            Tile1 = 1,
            Tile2 = 2,
            Tile3 = 3,
            Tile4 = 4,
            Tile5 = 5,
            Tile6 = 6,
            Tile7 = 7,
            Tile8 = 8,
            TileEmpty = 0,
            TileFlag = 10,
            TileMine = 20,
            TileExploded = 30
        }
        #endregion

        public MainWindow() {
            InitializeComponent();
            GenerateGrid();
        }

        private void DrawGrid() {
            GameGrid.Children.Clear();
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {

                    if (GameGridExplored[r, c] == true) {
                        BitmapImage bitmapImage = new BitmapImage(new Uri($"Assets/{(TileValues)GameGridValues[r, c]}.png", UriKind.Relative));
                        Image img = new Image {
                            Source = bitmapImage
                        };
                        GameGrid.Children.Add(img);
                    } else if (GameGridExplored[r, c] == false) {
                        GameGrid.Children.Add(new Image { Source = new BitmapImage(new Uri("Assets/TileUnknown.png", UriKind.Relative)) });
                    } else if (GameGridExplored[r, c] == null) {
                        GameGrid.Children.Add(new Image { Source = new BitmapImage(new Uri("Assets/TileFlag.png", UriKind.Relative)) });
                    }


                }
            }
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
            GameGridValues = new int[rows, columns];
            GameGridExplored = new bool?[rows, columns];

            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {
                    GameGridExplored[r, c] = false;
                }
            }
        }

        private void GenerateBombs() {

            Random random = new Random();
            for (int i = 0; i < bombs; i++) {
                int x = random.Next(rows);
                int y = random.Next(columns);
                if (GameGridValues[x, y] == 20) {
                    i--;
                } else {
                    GameGridValues[x, y] = 20;
                }

            }
        }

        private void GenerateNumbers() {
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {

                    if (GameGridValues[r, c] == 20) {
                        continue;
                    }
                    int bombCount = 0;

                    try {
                        bombCount += GameGridValues[r - 1, c - 1] == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += GameGridValues[r - 1, c] == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += GameGridValues[r - 1, c + 1] == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += GameGridValues[r, c - 1] == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += GameGridValues[r, c + 1] == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += GameGridValues[r + 1, c - 1] == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += GameGridValues[r + 1, c] == 20 ? 1 : 0;
                    } catch (Exception) { }
                    try {
                        bombCount += GameGridValues[r + 1, c + 1] == 20 ? 1 : 0;
                    } catch (Exception) { }

                    GameGridValues[r, c] = bombCount;

                }
            }
        }
        #endregion

        #region Clicking
        private void PlaceFlag(object sender, MouseButtonEventArgs e) {
            Point Clickpos = e.GetPosition(GameGrid);

            Point ClickedTile = new Point(Math.Floor(Clickpos.X / size), Math.Floor(Clickpos.Y / size));

            if (GameGridExplored[(int)ClickedTile.Y, (int)ClickedTile.X] == null) {
                bombsRemaining++;
                FlagsRemaining.Text = bombsRemaining.ToString();
                GameGridExplored[(int)ClickedTile.Y, (int)ClickedTile.X] = false;
                DrawGrid();
                return;
            }

            if (GameGridExplored[(int)ClickedTile.Y, (int)ClickedTile.X] == true) {
                return;
            }

            bombsRemaining--;
            FlagsRemaining.Text = bombsRemaining.ToString();

            GameGridExplored[(int)ClickedTile.Y, (int)ClickedTile.X] = null;
            DrawGrid();

            if (GameGridExplored.Cast<bool?>().All(b => b == true)) {
                MessageBox.Show("YOU WIN!!!");
            }
        }

        private void ClickGameGrid(object sender, MouseEventArgs e) {

            Point Clickpos = e.GetPosition(GameGrid);
            Point ClickedTile = new Point(Math.Floor(Clickpos.X / size), Math.Floor(Clickpos.Y / size));

            int value = GameGridValues[(int)ClickedTile.Y, (int)ClickedTile.X];
            bool? explored = GameGridExplored[(int)ClickedTile.Y, (int)ClickedTile.X];

            GameGridExplored[(int)ClickedTile.Y, (int)ClickedTile.X] = true;

            switch (value) {

                case 0:
                    FloodFill(ClickedTile);
                    return;

                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    RevealNumbers(ClickedTile, value, explored);
                    DrawGrid();
                    return;

                case 20:
                    YouLose();
                    return;
            }
        }
        #endregion

        private void FloodFill(Point point) {
            List<Point> newPoints = new List<Point>();
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    try {
                        if (GameGridValues[Convert.ToInt32(Math.Round(point.Y)) + x, Convert.ToInt32(point.X) + y] == 0 && GameGridExplored[Convert.ToInt32(Math.Round(point.Y)) + x, Convert.ToInt32(point.X) + y] == false) {
                            newPoints.Add(new Point(point.X + y, point.Y + x));
                        }
                        if (GameGridExplored[Convert.ToInt32(Math.Round(point.Y)) + x, Convert.ToInt32(point.X) + y] == false) {
                            GameGridExplored[Convert.ToInt32(Math.Round(point.Y)) + x, Convert.ToInt32(point.X) + y] = true;
                        }

                    } catch (Exception) {
                    }
                }
            }
            foreach (Point newPoint in newPoints) {
                FloodFill(newPoint);
            }
            DrawGrid();
            CheckIfWon();
        }

        private void RevealNumbers(Point clickedTile, int value, bool? explored) {
            if (explored == true) {
                FlagRevealMethod(clickedTile, value);
            } else if (explored == false) {
                GameGridExplored[(int)Math.Round(clickedTile.Y), (int)Math.Round(clickedTile.X)] = true;
            }
            DrawGrid();
        }

        private void FlagRevealMethod(Point clickedTile, int value) {
            int flagCount = 0;
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    try {
                        if (GameGridExplored[(int)Math.Round(clickedTile.Y + y), (int)Math.Round(clickedTile.X + x)] == null) {
                            flagCount++;
                        }
                    } catch (Exception) { }
                }
            }
            if (flagCount == value) {
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        try {
                            
                            if (GameGridExplored[(int)Math.Round(clickedTile.Y + y), (int)Math.Round(clickedTile.X + x)] != null) {
                                if (GameGridValues[(int)Math.Round(clickedTile.Y + y), (int)Math.Round(clickedTile.X + x)] == 20) {
                                    YouLose();
                                    return;
                                }
                                GameGridExplored[(int)Math.Round(clickedTile.Y + y), (int)Math.Round(clickedTile.X + x)] = true;
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
            if (GameGridExplored.Cast<bool?>().Count(b => b == false) - bombsRemaining == 0) {
                MessageBox.Show("YOU WIN!!!");
            }
        }
        private void YouLose() {
            
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < columns; c++) {
                    GameGridExplored[r, c] = true;
                }
            }

            DrawGrid();
            MessageBox.Show("You Lose!");
        }
        #endregion
    }
}
