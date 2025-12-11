using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kincsvadaszok
{
    public partial class GameWindow : Window
    {
        private const int MapSize = 10;
        private const int NumberOfTreasures = 10; // Több kincs, hogy legyen verseny!

        // Játékos 1 (Zöld) - Bal felső sarok
        private int p1X = 0;
        private int p1Y = 0;
        public List<Treasure> LootP1 { get; private set; } = new List<Treasure>();

        // Játékos 2 (Kék) - Jobb alsó sarok
        private int p2X = MapSize - 1;
        private int p2Y = MapSize - 1;
        public List<Treasure> LootP2 { get; private set; } = new List<Treasure>();

        // Körkezelés
        private bool isPlayer1Turn = true; // Kezdésnek P1 jön

        // Pálya elemek
        private Border[,] mapCells;
        private Dictionary<(int, int), Treasure> treasureLocations = new Dictionary<(int, int), Treasure>();
        private Random random = new Random();

        //Játékos nevek 
        private string name1;
        private string name2; 

        public GameWindow(string p1Name, string p2Name)
        {
            this.name1 = p1Name;
            this.name2 = p2Name;

            InitializeComponent();

            InitializeMap();
            SpawnTreasures();

            // Kezdő állapot kirajzolása
            RenderMap();
            UpdateStatus();

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
                    Border cell = new Border
                    {
                        Background = Brushes.Black,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5)
                    };
                    GameGrid.Children.Add(cell);
                    mapCells[y, x] = cell;
                }
            }
        }

        private void SpawnTreasures()
        {
            int count = 0;
            while (count < NumberOfTreasures)
            {
                int rx = random.Next(MapSize);
                int ry = random.Next(MapSize);

                // Nem rakhatjuk a sarkokba (start pozíciók) és ahol már van
                bool isCorner = (rx == 0 && ry == 0) || (rx == MapSize - 1 && ry == MapSize - 1);

                if (!isCorner && !treasureLocations.ContainsKey((rx, ry)))
                {
                    Treasure t = new Treasure
                    {
                        Name = $"Kincs #{random.Next(100, 999)}",
                        Value = random.Next(10, 500)
                    };
                    treasureLocations.Add((rx, ry), t);
                    count++;
                }
            }
        }

        // --- MOZGÁS ÉS KÖRÖK KEZELÉSE ---
        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Meghatározzuk, ki a soros, és hol áll most
            int currentX = isPlayer1Turn ? p1X : p2X;
            int currentY = isPlayer1Turn ? p1Y : p2Y;

            // Kiszámoljuk az új helyet
            int newX = currentX;
            int newY = currentY;

            switch (e.Key)
            {
                case Key.Up: if (currentY > 0) newY--; break;
                case Key.Down: if (currentY < MapSize - 1) newY++; break;
                case Key.Left: if (currentX > 0) newX--; break;
                case Key.Right: if (currentX < MapSize - 1) newX++; break;
                case Key.Escape: this.Close(); return;
                default: return; // Ha nem nyíl, nem történik semmi
            }

            // Ha nem mozdult (falnak ment), akkor nem váltunk kört, próbálja újra
            if (newX == currentX && newY == currentY) return;

            // Ha a másik játékosra akarna lépni, azt nem engedjük (ütközés)
            if (isPlayer1Turn && newX == p2X && newY == p2Y) return;
            if (!isPlayer1Turn && newX == p1X && newY == p1Y) return;

            // --- LÉPÉS VÉGREHAJTÁSA ---

            // 1. Koordináták frissítése
            if (isPlayer1Turn) { p1X = newX; p1Y = newY; }
            else { p2X = newX; p2Y = newY; }

            // 2. Kincs felvétele az adott játékosnak
            if (treasureLocations.ContainsKey((newX, newY)))
            {
                Treasure found = treasureLocations[(newX, newY)];

                if (isPlayer1Turn) LootP1.Add(found);
                else LootP2.Add(found);

                treasureLocations.Remove((newX, newY));
            }

            // 3. Kör váltása (P1 -> P2, vagy P2 -> P1)
            isPlayer1Turn = !isPlayer1Turn;

            // 4. Képernyő frissítése
            RenderMap();
            UpdateStatus();

            // 5. Játék vége ellenőrzés
            CheckGameOver();
        }

        // --- GRAFIKA FRISSÍTÉSE ---
        // Ez a függvény felelős azért, hogy minden kocka a megfelelő színű legyen
        private void RenderMap()
        {
            for (int y = 0; y < MapSize; y++)
            {
                for (int x = 0; x < MapSize; x++)
                {
                    // Alap: Fekete
                    Brush color = Brushes.Black;

                    // Ha kincs van ott: Arany
                    if (treasureLocations.ContainsKey((x, y)))
                        color = Brushes.Gold;

                    // Ha P1 (Zöld) áll ott
                    if (x == p1X && y == p1Y)
                        color = Brushes.LimeGreen;

                    // Ha P2 (Kék) áll ott
                    if (x == p2X && y == p2Y)
                        color = Brushes.Cyan;

                    mapCells[y, x].Background = color;
                }
            }
        }

        private void UpdateStatus()
        {
            // Itt használjuk a neveket a "Player 1" helyett!
            string turnName = isPlayer1Turn ? $"{name1} (Zöld)" : $"{name2} (Kék)";

            if (txtStatus != null)
            {
                txtStatus.Text = $"KÖVETKEZIK: {turnName} | {name1}: {LootP1.Count} db | {name2}: {LootP2.Count} db | Pályán: {treasureLocations.Count}";
            }
        }

        private void CheckGameOver()
        {
            if (treasureLocations.Count == 0)
            {
                // Kényszerített frissítés a legutolsó lépés kirajzolásához
                Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                string winner = "";
                if (LootP1.Count > LootP2.Count) winner = "PLAYER 1 NYERT!";
                else if (LootP2.Count > LootP1.Count) winner = "PLAYER 2 NYERT!";
                else winner = "DÖNTETLEN!";

                MessageBox.Show($"Vége a játéknak!\n{winner}\n\nP1 gyűjtött: {LootP1.Count}\nP2 gyűjtött: {LootP2.Count}");
                this.Close();
            }
        }
    }
}