using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

namespace Kincsvadaszok
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {

        private const int MapSize = 10; //pályaméret

        private int playerX = 0;
        private int playerY = 0; //játékos pozicio

        private Border[,] mapCells; //határok, könnyű updateért

        public GameWindow()
        {
            InitializeComponent();
            InitializeMap();
            UpdatePlayerPosition();

            this.KeyDown += GameWindow_KeyDown;
            this.Focus();
        }

        private void InitializeMap()
        {
            mapCells = new Border[MapSize, MapSize];
            GameGrid.Children.Clear();

            for (int y = 0; y < MapSize; y++)
            {
                for (int x = 0; x < MapSize; x++)
                {
                    Border cell = new Border //szimpla border minden cellának 
                    {
                        Background = Brushes.Black,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5)
                    };

                    GameGrid.Children.Add(cell); //XAML-hez hozzáadás
                    mapCells[y, x] = cell;
                }
            }
        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            mapCells[playerY, playerX].Background = Brushes.Black;
            switch (e.Key)
            {
                case Key.Up:
                    if (playerY > 0) playerY--;
                    break;
                case Key.Down:
                    if (playerY < MapSize - 1) playerY++;
                    break;
                case Key.Left:
                    if (playerX > 0) playerX--;
                    break;
                case Key.Right:
                    if (playerX < MapSize - 1) playerX++;
                    break;
            }

            UpdatePlayerPosition();
        }


        private void UpdatePlayerPosition()
        {
            mapCells[playerY, playerX].Background = Brushes.LimeGreen; // a játékos cellája zöld

            if (txtStatus != null)
            {
                txtStatus.Text = $"Position: {playerX}, {playerY}";
            }
        }
    }
}
