using Kincsvadaszok.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Kincsvadaszok
{
    
    public partial class GameWindow : Window
    {
  
        private const int MapSize = 10;
        private const int NumberOfTreasures = 10; // Több kincs, hogy legyen verseny!
        private const int NumberOfObstacles = 15;
        private bool isGameFinished = false;


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
        private HashSet<(int, int)> obstacleLocations = new HashSet<(int, int)>();
        private Random random = new Random();

        //Játékos nevek 
        private string name1;
        private string name2;

        private ImageBrush player1Img;
        private ImageBrush player2Img;
        private ImageBrush lootImg;
        private ImageBrush floorImg;
        private ImageBrush wallImg;

        public GameWindow(string p1Name, string p2Name)
        {
            this.name1 = p1Name;
            this.name2 = p2Name;

            InitializeComponent();
            LoadImages();

            InitializeMap();
            SpawnObstacles();
            SpawnTreasures();

            // Kezdő állapot kirajzolása
            RenderMap();
            UpdateStatus();

            this.KeyDown += GameWindow_KeyDown;
            this.Focus();
        }
        public GameWindow(SavedGame state)
        {
            InitializeComponent();
            LoadImages(); // Képek betöltése

            // Adatok visszaállítása
            name1 = state.P1Name;
            name2 = state.P2Name;
            p1X = state.P1X; p1Y = state.P1Y;
            p2X = state.P2X; p2Y = state.P2Y;
            isPlayer1Turn = state.IsPlayer1Turn;
            LootP1 = state.LootP1;
            LootP2 = state.LootP2;

            InitializeMap(); // Üres rács létrehozása

            // Kincsek visszapakolása
            treasureLocations.Clear();
            foreach (var t in state.TreasureOnMap)
            {
                treasureLocations.Add((t.X, t.Y), t.Item);
            }

            // Falak visszapakolása
            obstacleLocations.Clear();
            foreach (var o in state.ObstaclesOnMap)
            {
                obstacleLocations.Add((o.X, o.Y));
            }

            // Kirajzolás
            RenderMap();
            UpdateStatus();

            this.KeyDown += GameWindow_KeyDown;
            this.Focus();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!isGameFinished)
            {
                var result = MessageBox.Show(
                    "A játék még folyamatban van! Szeretnéd menteni kilépés előtt?",
                    "Játék mentése",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (result == MessageBoxResult.Yes)
                {
                    SaveCurrentGame();
                }
            }
            base.OnClosing(e);
        }

        private void SaveCurrentGame()
        {
            //adatok osszegyujtese objektumba
            SavedGame state = new SavedGame 
            { 
                P1Name = name1, P2Name = name2,
                P1X = p1X, P1Y = p1Y,
                P2X = p2X, P2Y = p2Y,
                IsPlayer1Turn = isPlayer1Turn,
                LootP1 = LootP1, LootP2 = LootP2,
                TreasureOnMap = new List<TreasureData>(),
                ObstaclesOnMap = new List<PointData>()
            };

            //szotarat listava alakitunk hogy mentheto legyen
            foreach (var kvp in treasureLocations)
            {
                state.TreasureOnMap.Add(new TreasureData{
                    X = kvp.Key.Item1,
                    Y = kvp.Key.Item2,
                    Item = kvp.Value
                });
            }

            //falakat is listava
            foreach (var obs in obstacleLocations)
            {
                state.ObstaclesOnMap.Add(new PointData { X = obs.Item1, Y = obs.Item2});
            }

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Kincsvadászok mentés (*.sav)|*.sav";
            sfd.FileName =  $"mentes_{DateTime.Now:MMdd_HHmm}";

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    string json = System.Text.Json.JsonSerializer.Serialize(state);
                    System.IO.File.WriteAllText(sfd.FileName, json);
                    MessageBox.Show("Sikeres mentés!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba mentéskor: " + ex.Message);
                }
            }
        }

        private void LoadImages()
        {
            try
            {
                // Try absolute pack URI first
                var uri1 = new Uri("pack://application:,,,/Images/player1.png", UriKind.Absolute);
                player1Img = new ImageBrush(new BitmapImage(uri1));
                var uri2 = new Uri("pack://application:,,,/Images/player2.png",UriKind.Absolute);
                player2Img = new ImageBrush(new BitmapImage(uri2));
                var uri3 = new Uri("pack://application:,,,/Images/loot.png",UriKind.Absolute);
                lootImg = new ImageBrush(new BitmapImage(uri3));
                var uri4 = new Uri("pack://application:,,,/Images/cobblestone.jpg",UriKind.Absolute);
                floorImg = new ImageBrush(new BitmapImage(uri4));
                GameGrid.Background = floorImg;
                var uri5 = new Uri("pack://application:,,,/Images/wall.jpg",UriKind.Absolute);
                wallImg = new ImageBrush(new BitmapImage(uri5));
            }
            catch
            {
                // Fallback: try relative path
                try
                {
                    var uri1 = new Uri("/Images/player1.png", UriKind.Relative);
                    player1Img = new ImageBrush(new BitmapImage(uri1));
                    var uri2 = new Uri("/Images/player2.png",UriKind.Relative);
                    player2Img = new ImageBrush(new BitmapImage(uri2));
                    var uri3 = new Uri("Images/loot.png",UriKind.Relative);
                    lootImg = new ImageBrush(new BitmapImage(uri3));
                }
                catch
                {
                    MessageBox.Show("Hiba a képek betöltésekor! Ellenőrizd, hogy az Images/player1.png létezik a projektben, és a Build Action 'Resource' értékre van állítva.");
                }
            }
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
                        Background = Brushes.Transparent
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

                bool isCorner = (rx == 0 && ry == 0) || (rx == MapSize - 1 && ry == MapSize - 1);

                // MÓDOSÍTVA: Csak akkor rakjuk le, ha nincs ott FAL sem!
                if (!isCorner && !treasureLocations.ContainsKey((rx, ry)) && !obstacleLocations.Contains((rx, ry)))
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

        private void SpawnObstacles() // Falak lerakása
        {
            int count = 0;
            while (count < NumberOfObstacles)
            {
                int rx = random.Next(MapSize);
                int ry = random.Next(MapSize);

                bool isStartPos = (rx == 0 && ry == 0) || (rx == MapSize - 1 && ry == MapSize - 1); //startpoziciokra nem rakunk falat 

                if (!isStartPos && !obstacleLocations.Contains((rx, ry))) //ha nem startpozi és még nincsen ott fal 
                {
                    obstacleLocations.Add((rx, ry));
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

            if (obstacleLocations.Contains((newX, newY)))
            {
                return;
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

        // Ez a függvény felelős azért, hogy minden kocka a megfelelő színű legyen
        private void RenderMap()
        {
            for (int y = 0; y < MapSize; y++)
            {
                for (int x = 0; x < MapSize; x++)
                {
                    // 1. ALAPÉRTELMEZÉS: A padló képe (ha van), különben fekete
                    Brush finalBrush = Brushes.Transparent;

                    // 2. FAL (felülírja a padlót)
                    if (obstacleLocations.Contains((x, y)))
                    {
                        finalBrush = wallImg;
                    }

                    // 3. KINCS (felülírja a padlót)
                    if (treasureLocations.ContainsKey((x, y)))
                    {
                        finalBrush = (Brush)lootImg ?? Brushes.Gold;
                    }

                    // 4. JÁTÉKOS 1 (felülír mindent)
                    if (x == p1X && y == p1Y)
                    {
                        finalBrush = (Brush)player1Img ?? Brushes.LimeGreen;
                    }

                    // 5. JÁTÉKOS 2 (felülír mindent)
                    if (x == p2X && y == p2Y)
                    {
                        finalBrush = (Brush)player2Img ?? Brushes.Cyan;
                    }

                    // VÉGSŐ RENDERELÉS
                    mapCells[y, x].Background = finalBrush;
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
                isGameFinished = true;
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