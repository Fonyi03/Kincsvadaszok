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
        // a palya meretei es a jatek szabalyai
        private const int MapSize = 10;
        private const int NumberOfTreasures = 10; // tobb kincs, hogy legyen verseny!
        private const int NumberOfObstacles = 15;
        private bool isGameFinished = false; // ez jelzi ha vege van h ne kerdezzen ra a mentesre
        private int totalSteps = 0;
        private const int MaxSteps = 100;

        // ezt nezi a mainwindow h dontetlen lett e a lepesek miatt
        public bool IsDrawBySteps { get; set; } = false;


        // jatekos 1 (zold) - bal felso sarok
        private int p1X = 0;
        private int p1Y = 0;
        public List<Treasure> LootP1 { get; private set; } = new List<Treasure>(); // itt gyujti a kincseit

        // jatekos 2 (kek) - jobb also sarok
        private int p2X = MapSize - 1;
        private int p2Y = MapSize - 1;
        public List<Treasure> LootP2 { get; private set; } = new List<Treasure>();

        // korkezeles
        private bool isPlayer1Turn = true; // kezdesnek p1 jon

        // palya elemek tarolasa
        private Border[,] mapCells; // ez a vizualis racs
        private Dictionary<(int, int), Treasure> treasureLocations = new Dictionary<(int, int), Treasure>(); // hol vannak a kincsek
        private HashSet<(int, int)> obstacleLocations = new HashSet<(int, int)>(); // hol vannak a falak
        private Random random = new Random();

        // jatekos nevek 
        private string name1;
        private string name2;

        // kepek tarolasa h ne kelljen mindig ujratolteni oket
        private ImageBrush player1Img;
        private ImageBrush player2Img;
        private ImageBrush lootImg;
        private ImageBrush floorImg;
        private ImageBrush wallImg;

        // alap konstruktor amikor uj jatekot inditunk
        public GameWindow(string p1Name, string p2Name)
        {
            this.name1 = p1Name;
            this.name2 = p2Name;

            InitializeComponent();
            LoadImages(); // kepek betoltese

            InitializeMap(); // palya felepitese
            SpawnObstacles(); // akadalyok lerakasa
            SpawnTreasures(); // kincsek lerakasa

            // kezdo allapot kirajzolasa
            RenderMap();
            UpdateStatus();

            this.KeyDown += GameWindow_KeyDown; // gombnyomas figyeles
            this.Focus();
        }

        // ez a konstruktor tolti be a mentett allast
        public GameWindow(SavedGame state)
        {
            InitializeComponent();
            LoadImages(); // kepek betoltese

            // adatok visszaallitasa a mentesbol
            name1 = state.P1Name;
            name2 = state.P2Name;
            p1X = state.P1X; p1Y = state.P1Y;
            p2X = state.P2X; p2Y = state.P2Y;
            isPlayer1Turn = state.IsPlayer1Turn;
            LootP1 = state.LootP1;
            LootP2 = state.LootP2;
            totalSteps = state.TotalSteps; // a lepeseket is visszaallitjuk

            InitializeMap(); // ures racs letrehozasa

            // kincsek visszapakolasa a regi helyukre
            treasureLocations.Clear();
            foreach (var t in state.TreasureOnMap)
            {
                treasureLocations.Add((t.X, t.Y), t.Item);
            }

            // falak visszapakolasa
            obstacleLocations.Clear();
            foreach (var o in state.ObstaclesOnMap)
            {
                obstacleLocations.Add((o.X, o.Y));
            }

            // kirajzolas
            RenderMap();
            UpdateStatus();

            this.KeyDown += GameWindow_KeyDown;
            this.Focus();
        }

        // ha bezarjuk az ablakot es meg megy a jatek akkor rakerdez h mentsunk e
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
                    e.Cancel = true; // megsem lepunk ki
                    return;
                }

                if (result == MessageBoxResult.Yes)
                {
                    SaveCurrentGame(); // mentjuk a jatekot
                }
            }
            base.OnClosing(e);
        }

        // ez vegzi a mentes logikajat json fajlba
        private void SaveCurrentGame()
        {
            // adatok osszegyujtese objektumba
            SavedGame state = new SavedGame
            {
                P1Name = name1,
                P2Name = name2,
                P1X = p1X,
                P1Y = p1Y,
                P2X = p2X,
                P2Y = p2Y,
                IsPlayer1Turn = isPlayer1Turn,
                LootP1 = LootP1,
                LootP2 = LootP2,
                TreasureOnMap = new List<TreasureData>(),
                ObstaclesOnMap = new List<PointData>(),
                TotalSteps = totalSteps
            };

            // szotarat listava alakitunk hogy mentheto legyen
            foreach (var kvp in treasureLocations)
            {
                state.TreasureOnMap.Add(new TreasureData
                {
                    X = kvp.Key.Item1,
                    Y = kvp.Key.Item2,
                    Item = kvp.Value
                });
            }

            // falakat is listavaalakitjuk
            foreach (var obs in obstacleLocations)
            {
                state.ObstaclesOnMap.Add(new PointData { X = obs.Item1, Y = obs.Item2 });
            }

            // felugro ablak h hova mentsuk
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Kincsvadászok mentés (*.sav)|*.sav";
            sfd.FileName = $"mentes_{DateTime.Now:MMdd_HHmm}";

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    // szerializalas es iras
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

        // beolvassuk a kepeket h szep legyen a jatek
        private void LoadImages()
        {
            try
            {
                // eloszor abszolut utvonallal probaljuk
                var uri1 = new Uri("pack://application:,,,/Images/player1.png", UriKind.Absolute);
                player1Img = new ImageBrush(new BitmapImage(uri1));
                var uri2 = new Uri("pack://application:,,,/Images/player2.png", UriKind.Absolute);
                player2Img = new ImageBrush(new BitmapImage(uri2));
                var uri3 = new Uri("pack://application:,,,/Images/loot.png", UriKind.Absolute);
                lootImg = new ImageBrush(new BitmapImage(uri3));
                var uri4 = new Uri("pack://application:,,,/Images/cobblestone.jpg", UriKind.Absolute);
                floorImg = new ImageBrush(new BitmapImage(uri4));
                GameGrid.Background = floorImg;
                var uri5 = new Uri("pack://application:,,,/Images/wall.jpg", UriKind.Absolute);
                wallImg = new ImageBrush(new BitmapImage(uri5));
            }
            catch
            {
                // ha nem megy akkor relativ utvonallal
                try
                {
                    var uri1 = new Uri("/Images/player1.png", UriKind.Relative);
                    player1Img = new ImageBrush(new BitmapImage(uri1));
                    var uri2 = new Uri("/Images/player2.png", UriKind.Relative);
                    player2Img = new ImageBrush(new BitmapImage(uri2));
                    var uri3 = new Uri("Images/loot.png", UriKind.Relative);
                    lootImg = new ImageBrush(new BitmapImage(uri3));
                }
                catch
                {
                    MessageBox.Show("Hiba a képek betöltésekor! Ellenőrizd, hogy az Images/player1.png létezik a projektben, és a Build Action 'Resource' értékre van állítva.");
                }
            }
        }

        // letrehozzuk a racsot a kepernyon
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

        // random lerakjuk a kincseket
        private void SpawnTreasures()
        {
            int count = 0;
            while (count < NumberOfTreasures)
            {
                int rx = random.Next(MapSize);
                int ry = random.Next(MapSize);

                bool isCorner = (rx == 0 && ry == 0) || (rx == MapSize - 1 && ry == MapSize - 1);

                // modositva: csak akkor rakjuk le, ha nincs ott fal sem es nem sarok
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

        // falak random lerakasa
        private void SpawnObstacles()
        {
            int count = 0;
            while (count < NumberOfObstacles)
            {
                int rx = random.Next(MapSize);
                int ry = random.Next(MapSize);

                bool isStartPos = (rx == 0 && ry == 0) || (rx == MapSize - 1 && ry == MapSize - 1); // startpoziciokra nem rakunk falat 

                if (!isStartPos && !obstacleLocations.Contains((rx, ry))) // ha nem startpozi és még nincsen ott fal 
                {
                    obstacleLocations.Add((rx, ry));
                    count++;
                }

            }
        }

        // --- mozgas es korok kezelese ---
        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { this.Close(); return; }

            int currentX = isPlayer1Turn ? p1X : p2X;
            int currentY = isPlayer1Turn ? p1Y : p2Y;

            // A másik játékos pozíciója (az ütközéshez kell)
            int otherX = isPlayer1Turn ? p2X : p1X;
            int otherY = isPlayer1Turn ? p2Y : p1Y;

            // --- 1. ÚJ POZÍCIÓ KISZÁMÍTÁSA (GameLogic használata) ---
            // Figyeld meg: most már nem itt vannak az if-ek, hanem a GameLogic-ban!

            // Szűrés: P1 csak WASD, P2 csak Nyilak
            if (isPlayer1Turn && (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)) return;
            if (!isPlayer1Turn && (e.Key == Key.W || e.Key == Key.S || e.Key == Key.A || e.Key == Key.D)) return;

            var (newX, newY) = GameLogic.CalculateNewPosition(currentX, currentY, e.Key, MapSize);

            // --- 2. ÉRVÉNYESSÉG ELLENŐRZÉSE (GameLogic használata) ---
            if (!GameLogic.isValidMove(newX, newY, obstacleLocations, otherX, otherY))
            {
                return; // Ha érvénytelen, nem lépünk
            }

            if (newX == currentX && newY == currentY) return; // Ha nem mozdult
        }


            // ez a fuggveny felelos azert, hogy minden kocka a megfelelo szinu legyen
        private void RenderMap()
        {
            for (int y = 0; y < MapSize; y++)
            {
                for (int x = 0; x < MapSize; x++)
                {
                    // 1. alapertelmezes: a padlo kepe (ha van), kulonben fekete
                    Brush finalBrush = Brushes.Transparent;

                    // 2. fal (felulirja a padlot)
                    if (obstacleLocations.Contains((x, y)))
                    {
                        finalBrush = wallImg;
                    }

                    // 3. kincs (felulirja a padlot)
                    if (treasureLocations.ContainsKey((x, y)))
                    {
                        finalBrush = (Brush)lootImg ?? Brushes.Gold;
                    }

                    // 4. jatekos 1 (felulir mindent)
                    if (x == p1X && y == p1Y)
                    {
                        finalBrush = (Brush)player1Img ?? Brushes.LimeGreen;
                    }

                    // 5. jatekos 2 (felulir mindent)
                    if (x == p2X && y == p2Y)
                    {
                        finalBrush = (Brush)player2Img ?? Brushes.Cyan;
                    }

                    // vegso rendereles
                    mapCells[y, x].Background = finalBrush;
                }
            }
        }

        // frissitjuk a felso szoveget az aktualis adatokkal
        private void UpdateStatus()
        {
            string turnName = isPlayer1Turn ? $"{name1} (Zöld)" : $"{name2} (Kék)";
            if (txtStatus != null)
            {
                // hozzaadtuk a vegere a lepesszamlalot
                txtStatus.Text = $"KÖVETKEZIK: {turnName} | {name1}: {LootP1.Count} | {name2}: {LootP2.Count} | Pályán: {treasureLocations.Count} | LÉPÉSEK: {totalSteps}/{MaxSteps}";
            }
        }

        // megnezzuk vege van e a jateknak
        private void CheckGameOver()
        {
            if (treasureLocations.Count == 0)
            {
                isGameFinished = true;
                // kenyszeritett frissites a legutolso lepes kirajzolasahoz
                Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                // gyoztes kihirdetese a loot alapjan
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